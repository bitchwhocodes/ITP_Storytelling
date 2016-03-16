# STUDENT

{Shan Jin}

## Assignment 3

video here: 
https://vimeo.com/159250359

There 3 folders in this assignment:

server - I put 2 files under server folder onto Digital Ocean and run node server.js

mobile_client - use PhoneGap to put this project on my phone. Super simple tutorial here: http://phonegap.com/getstarted/
the mobile client would submit gyroscope data to server from time to time, it can also access photo album and choose a photo to upload.

unitysocket - this unity project uses socket to talk with server, get gyroscope data from phone and rotate the globe in response. It connects Google Geocode API to get the address of the place it's landing. It also creates new texture to show the image sent from mobile phone.