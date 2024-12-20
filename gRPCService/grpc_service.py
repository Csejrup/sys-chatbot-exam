import grpc
from concurrent import futures
from protos import chatbot_pb2_grpc, chatbot_pb2

# Implementation of the gRPC service
class ChatbotService(chatbot_pb2_grpc.ChatbotServiceServicer):
    def GetResponse(self, request, context):
        print(f"Received request: {request.message}")
        return chatbot_pb2.ChatbotResponse(message="Hello from Python gRPC!")

# gRPC server setup
def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    chatbot_pb2_grpc.add_ChatbotServiceServicer_to_server(ChatbotService(), server)
    server.add_insecure_port("[::]:50051")
    print("gRPC server is running on port 50051...")
    server.start()
    server.wait_for_termination()

if __name__ == "__main__":
    serve()
