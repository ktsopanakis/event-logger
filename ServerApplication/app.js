#!/bin/env node

var express = require('express')
  , fs = require('fs')
  , path = require('path')
  , mongoose = require("mongoose")
  , config = require("./config/config.js")["production"]
  , socketio = require("socket.io")
  , ejs = require('ejs')
  , logger = require("morgan")
  , bodyparser = require("body-parser")
  , multer = require("multer");


// Bootstrap db connection
// Connect to mongodb
var connect = function () {
  var options = { server: { socketOptions: { keepAlive: 1 } } };
  mongoose.connect(config.db, options)
};
connect();
var db = mongoose.connection;
// Error handler
mongoose.connection.on('error', function (err) {
  console.log(err)
});

// Reconnect when closed
mongoose.connection.on('disconnected', function () {
  connect()
});

var app = express();
app.use(logger('dev'));
app.use(bodyparser.json());
app.use(bodyparser.urlencoded({extended:true}));
app.use(multer());
app.set('views', path.join(__dirname, 'views'));
app.set('view engine', 'jade');

db.once('open', function () {

  var expressServer = app.listen(config.port, config.domain, function () {
    console.log('Express server listening on port ' + config.port);
  });
  // bind our socket.io server to our express http server
  
  var io = socketio.listen(expressServer);
  
  expressServer.setTimeout(0, function (socket) {
    console.log("timeout occured");
    console.log("on socket: ", socket);
  });
  
  app.get("/test/", function(req, res){
    res.render("test.jade");
  });
  
  app.get("/sockets.js", function (req, res) {
    console.log("sockets request");
    res.setHeader('content-type', 'text/javascript');
    res.render('ejs/sockets.ejs', {server: config.domain, port: config.port});
  });
  // identifier / payload / timestamp
  app.post("/event-logger/:hospital/", function(req, res){
    console.log("received post request for %s, with: ", req.params.hospital, req.body);
    var collection = db.collection(req.params.hospital);
    var payload = req.body.payload || "";
    var identifier = req.body.identifier || "";
    var newEntry = {
      timestamp : req.body.timestamp,
      isRead: false,
      receivedTime: Math.round(+new Date()/1000)
    };
    try{
      payload = JSON.parse(req.body.payload);
    }catch(e){
      console.log("payload failed to get parsed");
    }
    try{
      identifier = JSON.parse(req.body.identifier);
    }catch(e){
      console.log("identifier failed to get parsed");
    }
    newEntry["payload"] = payload;
    newEntry["identifier"] = identifier;
    newEntry["title"] = "";
    newEntry["origin"] = req.params.hospital;
    collection.insert(newEntry, function(err){
      if(err) {console.log(err);}
      collection.find({"isRead": false}, function(err, data){
        if(err) {console.log("error in find: ", err);}
        data.toArray(function(err, unreadLogs){
          io.emit("unreadErrorMessages", {unread: unreadLogs});
          res.send("ok");
        });
      });
    });
  });
  
  io.sockets.on("connection", function(socket){
    console.log("openned websocket");
    socket.on("ping", function(d){
      console.log("received ping: ", d);
      socket.emit("pong", {});
    });
  });
  exports.io = io;
});