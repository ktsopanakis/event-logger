module.exports = {
  production: {
    db: 'mongodb://admin:admin01@127.0.0.1/event-logs',
    db_domain: '127.0.0.1',
    db_name: 'event-logs',
    db_port: 27017,
    db_username: 'admin',
    db_password: 'admin01',
    app: {
      name: 'Event Logger'
    },
    port: 29017,
    authEnabled: true,
    domain: '127.0.0.1'
  }
};
