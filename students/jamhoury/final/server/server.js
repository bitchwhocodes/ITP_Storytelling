// HTTP Portion
var http = require('http');
var fs = require('fs'); // Using the filesystem module
var Twitter = require('twitter');

var httpServer = http.createServer(requestHandler);
httpServer.listen(8000);

var io = require('socket.io').listen(httpServer);

var twitter = new Twitter({
	consumer_key: '',
  	consumer_secret: '',
  	access_token_key: '',
  	access_token_secret: ''
});

function requestHandler(req, res) {
	// Read index.html
	fs.readFile(__dirname + req.url, 
		// Callback function for reading
		function (err, data) {
			// if there is an error
			if (err) {
				res.writeHead(500);
				return res.end('Error loading page');
			}
			// Otherwise, send the data, the contents of the file
			res.writeHead(200);
			res.end(data);
  		}
  	);
}


var stream = null;
var counter = 0;
var params = {track: 'migrant crisis, poverty, presidential election'};
// var params = {'locations':'-180,-90,180,90'};



io.sockets.on('connection', function (socket) {
		
	console.log("We have a new client: " + socket.id);

	if (stream == null) {
		// console.log('null');
		socket.on("start-tweets", function() {
			// console.log("start-tweets");
			twitter.stream('statuses/filter', params, function(s){
				// console.log("stream started");
				stream = s;
				// console.log("s");
		    	stream.on('data', function(tweet) {
		    		// console.log("stream on");
		    		//make tweet lowercase
		    		var textlow = tweet.text.toLowerCase();
		    		// console.log("got textlow");
		        	// If tweet has search term
	        	if (textlow.indexOf('presidential election') > -1) {
		            var outputPoint = {"text": textlow};
		            console.log("presidential");
		            io.emit("presidential", outputPoint);
	  		  	} else if (textlow.indexOf('migrant crisis') > -1) {
		            var outputPoint = {"text": textlow};
		            console.log("migrant");
		            io.emit("migrant", outputPoint);
	  		  	} else if (textlow.indexOf('poverty') > -1) {
		            var outputPoint = {"text": textlow};
		            console.log("poverty");
		            io.emit("poverty", outputPoint);
		        } else {
		    		counter += 1;    		
		    		console.log(counter);
		    		}
		    	});
			});
		});
	}

	//Emits signal to the client that they are connected 
	socket.emit("connected");
	
	socket.on('disconnect', function() {
		console.log("Client has disconnected " + socket.id);
	});
});










