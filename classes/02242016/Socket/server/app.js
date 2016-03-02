var app = require('http').createServer(handler)
var io = require('socket.io')(app);
var fs = require('fs');

app.listen(3000);

function handler (req, res) {
  fs.readFile(__dirname + '/index.html',
  function (err, data) {
    if (err) {
      res.writeHead(500);
      return res.end('Error loading index.html');
    }

    res.writeHead(200);
    res.end(data);
  });
}

io.on('connection', function (socket) {
    
  setInterval(function(){
      var num = getRandomArbitrary(0,100);
      socket.emit('randomnumber',{data:num.toString()});
      
  },5000);
  socket.on('randomnumberdone',function(data){
      console.log(data);
  })
  socket.emit('sendname', { name: 'Stacey' });
  socket.on('saymyname', function (data) {
    console.log(data);
  });
  
});

function getRandomArbitrary(min, max) {
    return Math.random() * (max - min) + min;
}