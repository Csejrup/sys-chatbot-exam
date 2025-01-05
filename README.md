docker build ./SysChatBot -f SysChatBot/SysChatBot.API/Dockerfile -t sys-chatbot-exam/apigateway
docker build ./SysChatBot -f SysChatBot/LogChatService/Dockerfile -t sys-chatbot-exam/logservice
docker build ./SysChatBot -f SysChatBot/AuthenticationService/Dockerfile -t sys-chatbot-exam/authenticationservice
docker build ./SysChatBot -f SysChatBot/ChatService/Dockerfile -t sys-chatbot-exam/chatservice
docker build . -f gRPCService/Dockerfile -t sys-chatbot-exam/grpcservice
