
# gRPCService

This is a Python-based gRPC service that implements a simple chatbot. The service responds to requests with a predefined message.

## Folder Structure

```
gRPCService/
│
├── protos/
│   ├── chatbot.proto           # The .proto file for gRPC definitions
│   ├── chatbot_pb2.py          # Auto-generated Python classes from .proto
│   ├── chatbot_pb2_grpc.py     # Auto-generated gRPC classes from .proto
│
├── grpc_service.py             # Main gRPC service implementation
├── requirements.txt            # Python dependencies
├── Dockerfile                  # Docker configuration
└── README.md                   # Instructions for running the service
```

---

## Setup and Run Instructions

### Prerequisites

- Python 3.9 or later
- `pip` package manager
- `virtualenv` (optional but recommended)

---

### Running Without Docker

1**Set Up a Virtual Environment (Optional)**
   ```bash
   python -m venv venv
   source venv/bin/activate   # On Windows: venv\Scripts\activate
   ```

2**Install Dependencies**
   ```bash
   pip install -r requirements.txt
   ```

3**Generate gRPC Files**
   If the `chatbot_pb2.py` and `chatbot_pb2_grpc.py` files are missing, generate them using:
   ```bash
   python -m grpc_tools.protoc -Iprotos --python_out=protos --grpc_python_out=protos protos/chatbot.proto
   ```

4**Run the Service**
   ```bash
   python grpc_service.py
   ```
   The server will start and listen on port `50051`.

---

### Testing the Service

Use the following Python client to test the gRPC service:

```python
import grpc
from protos import chatbot_pb2, chatbot_pb2_grpc

def run():
    with grpc.insecure_channel("localhost:50051") as channel:
        stub = chatbot_pb2_grpc.ChatbotServiceStub(channel)
        response = stub.GetResponse(chatbot_pb2.ChatbotRequest(message="Hello from client!"))
        print(f"Chatbot response: {response.message}")

if __name__ == "__main__":
    run()
```

Save this as `test_client.py` in the same directory and run:
```bash
python test_client.py
```

---

### Running with Docker

1. **Build the Docker Image**
   ```bash
   docker build -t grpc_service .
   ```

2. **Run the Docker Container**
   ```bash
   docker run -p 50051:50051 grpc_service
   ```

3. **Test the Service**
   Use the same `test_client.py` to test the service by connecting to `localhost:50051`.

---

## Notes

- Ensure the `chatbot.proto` file and its generated files are consistent across the client and server.
