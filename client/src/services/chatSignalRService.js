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

      // Connection lifecycle events
      this.connection.onreconnecting((error) => {
        console.warn("🔄 Reconnecting...", error);
        this.isConnected = false;
      });

      this.connection.onreconnected((connectionId) => {
        console.log("✅ Reconnected:", connectionId);
        this.isConnected = true;
      });

      this.connection.onclose((error) => {
        console.error("❌ Connection Closed:", error);
        this.isConnected = false;
      });

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
  async joinRoom(roomId) {
    if (!this.connection) throw new Error("Not connected");
    return this.connection.invoke("JoinRoom", roomId);
  }

  async leaveRoom(roomId) {
    if (!this.connection) throw new Error("Not connected");
    return this.connection.invoke("LeaveRoom", roomId);
  }

  /**
   * Gửi tin nhắn vào Room
   * @param {number} roomId - ID phòng
   * @param {string} message - Nội dung tin nhắn
   * @param {string[]|null} imageUrls - Danh sách URL ảnh
   * @param {number|null} replyToMessageId - ID tin nhắn được reply (optional)
   */
  async sendMessageToRoom(
    roomId,
    message,
    imageUrls = null,
    replyToMessageId = null,
  ) {
    if (!this.connection) throw new Error("Not connected");
    return this.connection.invoke(
      "SendMessageToRoom",
      roomId,
      message,
      imageUrls,
      replyToMessageId,
    );
  }

  async editMessageInRoom(messageId, roomId, newMessage, imageUrls = null) {
    if (!this.connection) throw new Error("Not connected");
    return this.connection.invoke(
      "EditMessageInRoom",
      messageId,
      roomId,
      newMessage,
      imageUrls,
    );
  }

  async deleteMessageInRoom(messageId, roomId) {
    if (!this.connection) throw new Error("Not connected");
    return this.connection.invoke("DeleteMessageInRoom", messageId, roomId);
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

  /**
   * Gửi tin nhắn vào Breakroom
   * @param {number} breakroomId - ID phòng con
   * @param {string} message - Nội dung tin nhắn
   * @param {string[]|null} imageUrls - Danh sách URL ảnh
   * @param {number|null} replyToMessageId - ID tin nhắn được reply (optional)
   */
  async sendMessageToBreakroom(
    breakroomId,
    message,
    imageUrls = null,
    replyToMessageId = null,
  ) {
    if (!this.connection) throw new Error("Not connected");
    return this.connection.invoke(
      "SendMessageToBreakroom",
      breakroomId,
      message,
      imageUrls,
      replyToMessageId,
    );
  }

  async editMessageInBreakroom(
    messageId,
    breakroomId,
    newMessage,
    imageUrls = null,
  ) {
    if (!this.connection) throw new Error("Not connected");
    return this.connection.invoke(
      "EditMessageInBreakroom",
      messageId,
      breakroomId,
      newMessage,
      imageUrls,
    );
  }

  async deleteMessageInBreakroom(messageId, breakroomId) {
    if (!this.connection) throw new Error("Not connected");
    return this.connection.invoke(
      "DeleteMessageInBreakroom",
      messageId,
      breakroomId,
    );
  }

  // ==================== P2P CHAT ====================
  /**
   * Gửi tin nhắn P2P
   * @param {number} receiverId - ID người nhận
   * @param {string} message - Nội dung tin nhắn
   * @param {string[]|null} imageUrls - Danh sách URL ảnh
   * @param {number|null} replyToMessageId - ID tin nhắn được reply (optional)
   */
  async sendMessageP2P(
    receiverId,
    message,
    imageUrls = null,
    replyToMessageId = null,
  ) {
    if (!this.connection) throw new Error("Not connected");
    return this.connection.invoke(
      "SendMessageP2P",
      receiverId,
      message,
      imageUrls,
      replyToMessageId,
    );
  }

  async editMessageP2P(messageId, newMessage, imageUrls = null) {
    if (!this.connection) throw new Error("Not connected");
    return this.connection.invoke(
      "EditMessageP2P",
      messageId,
      newMessage,
      imageUrls,
    );
  }

  async deleteMessageP2P(messageId, receiverId) {
    if (!this.connection) throw new Error("Not connected");
    return this.connection.invoke("DeleteMessageP2P", messageId, receiverId);
  }

  // ==================== REACTIONS ====================
  /**
   * Toggle reaction cho tin nhắn trong Room
   * @param {number} roomId - ID phòng
   * @param {number} messageId - ID tin nhắn
   * @param {string} emoji - Emoji reaction (👍, ❤️, 😂, etc.)
   */
  async toggleReactionInRoom(roomId, messageId, emoji) {
    if (!this.connection) throw new Error("Not connected");
    return this.connection.invoke(
      "ToggleReactionInRoom",
      roomId,
      messageId,
      emoji,
    );
  }

  /**
   * Lấy danh sách reactions của tin nhắn trong Room
   * @param {number} messageId - ID tin nhắn
   */
  async getReactionsInRoom(messageId) {
    if (!this.connection) throw new Error("Not connected");
    return this.connection.invoke("GetReactionsInRoom", messageId);
  }

  /**
   * Toggle reaction cho tin nhắn trong Breakroom
   */
  async toggleReactionInBreakroom(breakroomId, messageId, emoji) {
    if (!this.connection) throw new Error("Not connected");
    return this.connection.invoke(
      "ToggleReactionInBreakroom",
      breakroomId,
      messageId,
      emoji,
    );
  }

  async getReactionsInBreakroom(messageId) {
    if (!this.connection) throw new Error("Not connected");
    return this.connection.invoke("GetReactionsInBreakroom", messageId);
  }

  /**
   * Toggle reaction cho tin nhắn P2P
   * @param {number} messageId - ID tin nhắn
   * @param {string} emoji - Emoji reaction
   */
  async toggleReactionP2P(messageId, emoji) {
    if (!this.connection) throw new Error("Not connected");
    return this.connection.invoke("ToggleReactionP2P", messageId, emoji);
  }

  async getReactionsP2P(messageId) {
    if (!this.connection) throw new Error("Not connected");
    return this.connection.invoke("GetReactionsP2P", messageId);
  }

  // ==================== PINNED MESSAGES ====================
  /**
   * Ghim tin nhắn trong Room
   */
  async pinMessageInRoom(roomId, messageId) {
    if (!this.connection) throw new Error("Not connected");
    return this.connection.invoke("PinMessageInRoom", roomId, messageId);
  }

  /**
   * Gỡ ghim tin nhắn trong Room
   */
  async unpinMessageInRoom(roomId, messageId) {
    if (!this.connection) throw new Error("Not connected");
    return this.connection.invoke("UnpinMessageInRoom", roomId, messageId);
  }

  /**
   * Lấy danh sách tin nhắn đã ghim trong Room
   */
  async getPinnedMessagesInRoom(roomId) {
    if (!this.connection) throw new Error("Not connected");
    return this.connection.invoke("GetPinnedMessagesInRoom", roomId);
  }

  /**
   * Ghim tin nhắn trong Breakroom
   */
  async pinMessageInBreakroom(breakroomId, messageId) {
    if (!this.connection) throw new Error("Not connected");
    return this.connection.invoke(
      "PinMessageInBreakroom",
      breakroomId,
      messageId,
    );
  }

  async unpinMessageInBreakroom(breakroomId, messageId) {
    if (!this.connection) throw new Error("Not connected");
    return this.connection.invoke(
      "UnpinMessageInBreakroom",
      breakroomId,
      messageId,
    );
  }

  async getPinnedMessagesInBreakroom(breakroomId) {
    if (!this.connection) throw new Error("Not connected");
    return this.connection.invoke("GetPinnedMessagesInBreakroom", breakroomId);
  }

  // ==================== FORWARD MESSAGES ====================
  /**
   * Forward tin nhắn đến Room
   * @param {number} sourceMessageId - ID tin nhắn gốc
   * @param {string} sourceType - Loại nguồn: "room", "p2p", "breakroom"
   * @param {number} sourceId - ID nguồn (roomId, receiverId, breakroomId)
   * @param {number} targetRoomId - ID phòng đích
   */
  async forwardToRoom(sourceMessageId, sourceType, sourceId, targetRoomId) {
    if (!this.connection) throw new Error("Not connected");
    return this.connection.invoke(
      "ForwardToRoom",
      sourceMessageId,
      sourceType,
      sourceId,
      targetRoomId,
    );
  }

  /**
   * Forward tin nhắn đến P2P
   * @param {number} sourceMessageId - ID tin nhắn gốc
   * @param {string} sourceType - Loại nguồn: "room", "p2p", "breakroom"
   * @param {number} sourceId - ID nguồn
   * @param {number} targetReceiverId - ID người nhận
   */
  async forwardToP2P(sourceMessageId, sourceType, sourceId, targetReceiverId) {
    if (!this.connection) throw new Error("Not connected");
    return this.connection.invoke(
      "ForwardToP2P",
      sourceMessageId,
      sourceType,
      sourceId,
      targetReceiverId,
    );
  }

  /**
   * Forward tin nhắn đến Breakroom
   */
  async forwardToBreakroom(
    sourceMessageId,
    sourceType,
    sourceId,
    targetBreakroomId,
  ) {
    if (!this.connection) throw new Error("Not connected");
    return this.connection.invoke(
      "ForwardToBreakroom",
      sourceMessageId,
      sourceType,
      sourceId,
      targetBreakroomId,
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

  onMessageEdited(callback) {
    if (this.connection) {
      // Backend returns: { Id, Message, UpdatedDate, IsEdited, ReplyToMessageId, ... }
      this.connection.on("MessageEdited", callback);
    }
  }

  onMessageDeleted(callback) {
    if (this.connection) {
      // Backend returns: { MessageId }
      this.connection.on("MessageDeleted", callback);
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

  // ==================== REACTION EVENT LISTENERS ====================
  /**
   * Lắng nghe khi có reaction được thêm/xóa/cập nhật
   * Backend returns: { MessageId, Action, Emoji, UserId, Reactions: [...] }
   */
  onReactionUpdated(callback) {
    if (this.connection) {
      this.connection.on("ReactionUpdated", callback);
    }
  }

  /**
   * Lắng nghe khi load danh sách reactions
   * Backend returns: { MessageId, Reactions: [...], UserReactions: [...] }
   */
  onReactionsLoaded(callback) {
    if (this.connection) {
      this.connection.on("ReactionsLoaded", callback);
    }
  }

  // ==================== PIN EVENT LISTENERS ====================
  /**
   * Lắng nghe khi tin nhắn được ghim
   * Backend returns: { Id, RoomId/BreakroomId, MessageId, PinnedByUserId, Message: {...} }
   */
  onMessagePinned(callback) {
    if (this.connection) {
      this.connection.on("MessagePinned", callback);
    }
  }

  /**
   * Lắng nghe khi tin nhắn được gỡ ghim
   * Backend returns: { MessageId, RoomId/BreakroomId, UnpinnedByUserId }
   */
  onMessageUnpinned(callback) {
    if (this.connection) {
      this.connection.on("MessageUnpinned", callback);
    }
  }

  /**
   * Lắng nghe khi load danh sách tin nhắn đã ghim
   */
  onPinnedMessagesLoaded(callback) {
    if (this.connection) {
      this.connection.on("PinnedMessagesLoaded", callback);
    }
  }

  // ==================== CLEANUP ====================
  offAllListeners() {
    if (this.connection) {
      this.connection.off("ReceiveMessage");
      this.connection.off("ReceiveP2PMessage");
      this.connection.off("MessageEdited");
      this.connection.off("MessageDeleted");
      this.connection.off("UserJoined");
      this.connection.off("UserLeft");
      this.connection.off("UserTyping");
      this.connection.off("UserTypingP2P");
      this.connection.off("Error");
      // New listeners
      this.connection.off("ReactionUpdated");
      this.connection.off("ReactionsLoaded");
      this.connection.off("MessagePinned");
      this.connection.off("MessageUnpinned");
      this.connection.off("PinnedMessagesLoaded");
    }
  }

  getConnectionState() {
    return this.connection?.state ?? "Disconnected";
  }
}

export default new ChatSignalRService();
