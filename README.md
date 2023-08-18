# TCPTrainingListener
TCP socket client sample in C# to listen to neural network training events.  
When the training process is started, this sample is capable of connecting to the TCP server and listening to its events.
At the moment 4 types of messages are supported:  
- Iteration
- Predictions
- Statistics
- Error / Warning / Info (messages with text information that can be treated as logs)

# Launch  
Change the following line if the training server IP or port is different:  
https://github.com/rytisss/TCPTrainingListener/blob/13ccf8c91395e19f417ef5482d308707b27bd0ae/Example/CommandAndListeningSample.cs#L34-L36  

**NOTE! The training process must be launched!**
