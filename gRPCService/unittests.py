import pytest
from protos import chatbot_pb2, chatbot_pb2_grpc
from chatbot_service import ChatbotService  

@pytest.fixture
def chatbot_service():
    """Fixture to create a new instance of the ChatbotService"""
    return ChatbotService()

def test_get_response(chatbot_service):
    """Test the GetResponse method for a regular message"""
    # Create a mock request with a message
    request = chatbot_pb2.ChatbotRequest(message="Hello")

    # Call the service's GetResponse method
    response = chatbot_service.GetResponse(request, None)

    # Assert that the response message is as expected
    assert response.message == "Hello from Python gRPC!"

def test_get_response_empty_message(chatbot_service):
    """Test GetResponse with an empty message"""
    request = chatbot_pb2.ChatbotRequest(message="")
    response = chatbot_service.GetResponse(request, None)
    assert response.message == "Hello from Python gRPC!"

def test_get_response_special_characters(chatbot_service):
    """Test GetResponse with special characters in the message"""
    request = chatbot_pb2.ChatbotRequest(message="!@#$%^&*()")
    response = chatbot_service.GetResponse(request, None)
    assert response.message == "Hello from Python gRPC!"

