FROM python:3.9-slim

WORKDIR /gRPCService

# Install dependencies
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# Copy the application code
COPY . .

# Generate gRPC Python stubs
RUN python -m grpc_tools.protoc -I. --python_out=. --grpc_python_out=. protos/chatbot.proto

# Expose the gRPC port
EXPOSE 50051

# Run the gRPC service
CMD ["python", "grpc_service.py"]
# 