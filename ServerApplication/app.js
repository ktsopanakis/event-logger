#!/bin/env node

var express = require('express')
  , fs = require('fs')
  , path = require('path')
  , mongoose = require("mongoose");


// Bootstrap db connection
// Connect to mongodb
var connect = function () {
  var options = { server: { socketOptions: { keepAlive: 1 } } }
  mongoose.connect(config.db, options)
};
connect();

// Error handler
mongoose.connection.on('error', function (err) {
  console.log(err)
});

// Reconnect when closed
mongoose.connection.on('disconnected', function () {
  connect()
});

// Bootstrap models
var models_path = path.join(__dirname, 'app','models');
fs.readdirSync(models_path).forEach(function (file) {
  if (~file.indexOf('.js')) require(path.join(models_path, file))
});

var app = express();
// express settings
require('./config/express')(app, config);

// Bootstrap routes
require('./config/routes')(app);

app.configure(function(){

});

// Start the app by listening on <port>
var port;
if (env === 'production'){
  port = process.env.OPENSHIFT_NODEJS_PORT || 8080;
} else{
  port = process.env.PORT || 3000;
}

var ipaddress =  process.env.OPENSHIFT_NODEJS_IP || '127.0.0.1';

app.listen(port, ipaddress, function(){
  console.log('%s: Node server started on %s:%d ...', new Date(Date.now() ), ipaddress, port);
});

// expose app
exports = module.exports = app;
