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
  , multer = require("multer")
  , u = require("lodash-node");


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


app.use(function(req, res, next){
        req.text = '';
        req.setEncoding('utf8');
        req.on('data', function(chunk){ req.text += chunk });
        req.on('end', next);
});

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
    var setOfCollections = db.collection("set_of_log_collections");
    var total, unreadAppended, notRead, origins, read, readAppended;
    function renderTheUI(total, notRead, read){
      res.render("test.jade", {
        unreadMessages: notRead,
        readMessages: read,
        origins: total
      });
    }
    setOfCollections.find({}, function(err, colsCursor){
      if(err) {console.log("finding the set of collection for the GET status view threw: ", err);}
      colsCursor.toArray(function(err, colsArr){
        if(err) {console.log("converting the set of log collections cursor to array for the GET status view threw: ", err);}
        origins = colsArr;
        total = colsArr.length;
        unreadAppended = 0;
        readAppended = 0;
        notRead = [];
        read = [];
        if(total === 0) return renderTheUI(0, 0);
        u.each(colsArr, function(colObj){
          var currentCollection = db.collection(colObj.name);
          currentCollection.find({isRead: false}, function(err, unreadErrorsCursor){
            if(err) {console.log("gathering the unreadErrors for the GET status view threw: ", err);}
            unreadErrorsCursor.toArray(function(err, unreadErrorsArr){
              if(err) {console.log("converting the unreadErrorsCursor for the GET status view to array threw: ", err);}
              notRead = notRead.concat(unreadErrorsArr);
              unreadAppended += 1;
              if(total === unreadAppended && total === readAppended) return renderTheUI(origins, notRead, read);
            });
          });
          currentCollection.find({isRead: true}, function(err, readErrorCursor){
            if(err) {console.log("gathering the unreadErrors for the GET status view threw: ", err);}
            readErrorCursor.toArray(function(err, readErrorArr){
              if(err) {console.log("converting the readErrorCursor for the GET status view to array threw: ", err);}
              read = read.concat(readErrorArr);
              readAppended += 1;
              if(total === unreadAppended && total === readAppended) return renderTheUI(origins, notRead, read);
            });
          });
        });
      });
    });
    
  });
  
  app.get("/sockets.js", function (req, res) {
    console.log("sockets request");
    res.setHeader('content-type', 'text/javascript');
    res.render('ejs/sockets.ejs', {server: config.domain, port: config.port});
  });

  function returnUnreadErrorMessages(continuation){
    var setOfCollections = db.collection("set_of_log_collections");
    setOfCollections.find({}, function(err, cols){
      if(err) {console.log("set of log collections find all error: ", err);}
      cols.toArray(function(err, colsArr){
        if(err) {console.log("set of log collections to array conversion error: ", err)}
        if(colsArr === null || colsArr.length === 0) return continuation([]);
        var total = colsArr.length;
        var appended = 0;
        var notRead = [];
        u.each(colsArr, function(collection){
          var currentCollection = db.collection(collection.name);
          currentCollection.find({isRead: false}, function(err, unreadFromCurrentCursor){
            if(err) {console.log("find unread in collection threw err: ", err);}
            unreadFromCurrentCursor.toArray(function(err, ureadFromCurrentArr){
              if(err) {console.log("to array conversion threw err: ", err);}
              notRead = notRead.concat(ureadFromCurrentArr);
              appended += 1;
              if(total === appended) continuation(notRead);
            });
          });
        });
      });
    });
  }
  // identifier / payload / timestamp
  app.post("/event-logger/:hospital/", function(req, res){
    console.log("received post request for %s, with: ", req.params.hospital, req.body);
    var collection = db.collection(req.params.hospital);
    var setOfCollections = db.collection("set_of_log_collections");
    setOfCollections.find({name: req.params.hospital}, function(err, cols){
      if(err) {console.log("setOfCollections find error: ", err);}
      cols.toArray(function(err, colsArr){
        if(colsArr === null || colsArr.length === 0){
          setOfCollections.insert({name: req.params.hospital}, function(err){
            if(err) {console.log("setOfCollections insert in db error: ", err);}
          });
        }
      });
    });



    var jsonAvailable=false;
    var data=null;
    try{
        data=JSON.parse(req.text);
        jsonAvailable=true;
        req.body.payload = JSON.stringify(data.payload);
        req.body.identifier = JSON.stringify(data.identifier);
        req.body.timestamp = data.timestamp;
    }catch(err){

    }
      
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

    function broadcastUnread(unreadLogs){
      io.sockets.in("").emit("unreadErrorMessages", {unread: unreadLogs});
      res.send("ok");
    }
    
    collection.insert(newEntry, function(err){
      if(err) {console.log(err);}
      returnUnreadErrorMessages(broadcastUnread);
    });
    
  });

  function clientInitializerOn(socket){
    return function initializeClientList(unreadMessagesArray){
      socket.emit('initialList', {unread: unreadMessagesArray});
    }
  }
  
  
  io.sockets.on("connection", function(socket){
    console.log("openned websocket");
    
    returnUnreadErrorMessages(clientInitializerOn(socket));
    
    socket.on("ping", function(d){
      console.log("received ping: ", d);
      socket.emit("pong", {});
    });
    socket.on("messageRead", function(data){
      var collection = db.collection(data.origin);
      var BSON = require('mongodb').BSONPure;
      collection.find({_id: BSON.ObjectID(data.id)}, function(err, obj){
        if(err) {console.log("log find error: ", err);}
        obj.toArray(function(err, objArr){
          if(err) {console.log("find to array error: ", err);}
          var logToModify = objArr[0];
          console.log("logToModify: ", logToModify);
          logToModify["isRead"] = true;
          if(logToModify["readFrom"] === undefined) logToModify["readFrom"] = [];
          logToModify["readFrom"].push(data.readFrom);
          collection.update({_id: BSON.ObjectID(data.id)}, logToModify, function(err){
            if(err) {console.log("error upon log message modification: ", err);}
            socket.emit("messageReadOk", logToModify);
          });
        });
      });
    });
  });
  exports.io = io;
});