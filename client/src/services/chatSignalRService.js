import * as signalR from "@microsoft/signalr";

class ChatSignalRService {
  constructor() {
    this.connection = null;
    this.isConnected = false;
  }

  async startConnection(token, apiUrl) {
    try {
      this.connection = new signalR.HubConnectionBuilder()
        .withUrl(`${apiUrl}/hubs/chat`, {
          accessTokenFactory: () => token,
          transport:
            signalR.HttpTransportType.WebSockets |
            signalR.HttpTransportType.ServerSentEvents |
            signalR.HttpTransportType.LongPolling,
        })
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.Information)
        .build();

      await this.connection.start();
      this.isConnected = true;
      console.log("✅ SignalR Connected");
      return true;
    } catch (error) {
      console.error("❌ SignalR Connection Error:", error);
      this.isConnected = false;
      return false;
    }
  }

  async disconnect() {
    if (this.connection) {
      await this.connection.stop();
      this.isConnected = false;
      console.log("SignalR Disconnected");
    }
  }

  // ==================== ROOM CHAT ====================
  async joinRoom(roomId, password = null) {
    if (!this.connection) throw new Error("Not connected");

    return this.connection.invoke("JoinRoom", roomId);
  }

  async leaveRoom(roomId) {
    if (!this.connection) throw new Error("Not connected");

    return this.connection.invoke("LeaveRoom", roomId);
  }

  async sendMessageToRoom(roomId, message, imageUrls = null) {
    if (!this.connection) throw new Error("Not connected");

    return this.connection.invoke(
      "SendMessageToRoom",
      roomId,
      message,
      imageUrls
    );
  }

  // ==================== BREAKROOM CHAT ====================
  async joinBreakroom(breakroomId) {
    if (!this.connection) throw new Error("Not connected");

    return this.connection.invoke("JoinBreakroom", breakroomId);
  }

  async leaveBreakroom(breakroomId) {
    if (!this.connection) throw new Error("Not connected");

    return this.connection.invoke("LeaveBreakroom", breakroomId);
  }

  async sendMessageToBreakroom(breakroomId, message, imageUrls = null) {
    if (!this.connection) throw new Error("Not connected");

    return this.connection.invoke(
      "SendMessageToBreakroom",
      breakroomId,
      message,
      imageUrls
    );
  }

  // ==================== P2P CHAT ====================
  async sendMessageP2P(receiverId, message, imageUrls = null) {
    if (!this.connection) throw new Error("Not connected");

    return this.connection.invoke(
      "SendMessageP2P",
      receiverId,
      message,
      imageUrls
    );
  }

  // ==================== TYPING INDICATORS ====================
  async startTypingInRoom(roomId) {
    if (!this.connection) return;
    return this.connection.invoke("StartTypingInRoom", roomId);
  }

  async stopTypingInRoom(roomId) {
    if (!this.connection) return;
    return this.connection.invoke("StopTypingInRoom", roomId);
  }

  async startTypingP2P(receiverId) {
    if (!this.connection) return;
    return this.connection.invoke("StartTypingP2P", receiverId);
  }

  async stopTypingP2P(receiverId) {
    if (!this.connection) return;
    return this.connection.invoke("StopTypingP2P", receiverId);
  }

  // ==================== EVENT LISTENERS ====================
  onReceiveMessage(callback) {
    if (this.connection) {
      this.connection.on("ReceiveMessage", callback);
    }
  }

  onReceiveP2PMessage(callback) {
    if (this.connection) {
      this.connection.on("ReceiveP2PMessage", callback);
    }
  }

  onUserJoined(callback) {
    if (this.connection) {
      this.connection.on("UserJoined", callback);
    }
  }

  onUserLeft(callback) {
    if (this.connection) {
      this.connection.on("UserLeft", callback);
    }
  }

  onUserTyping(callback) {
    if (this.connection) {
      this.connection.on("UserTyping", callback);
    }
  }

  onUserTypingP2P(callback) {
    if (this.connection) {
      this.connection.on("UserTypingP2P", callback);
    }
  }

  onError(callback) {
    if (this.connection) {
      this.connection.on("Error", callback);
    }
  }
}

export default new ChatSignalRService();
