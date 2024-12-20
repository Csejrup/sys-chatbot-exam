FROM python:3.9-slim

WORKDIR /app

# Install dependencies
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# Copy application code
COPY . .

# Expose the gRPC port
EXPOSE 50051

# Run the gRPC service
CMD ["python", "grpc_service.py"]
