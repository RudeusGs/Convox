<template>
  <div class="chat-test">
    <!-- Header -->
    <div class="header">
      <div class="user-info">
        <h2>🎓 Convox Chat Test</h2>
        <span>User: {{ username }} (ID: {{ userId }})</span>
      </div>
      <div class="header-actions">
        <div class="status">
          <div :class="['status-dot', { connected: isConnected }]"></div>
          <span>{{ isConnected ? "Connected" : "Disconnected" }}</span>
        </div>
        <button class="btn-logout" @click="logout">Đăng Xuất</button>
      </div>
    </div>

    <!-- Main Content -->
    <div class="container">
      <!-- Sidebar -->
      <div class="sidebar">
        <!-- Room Chat -->
        <div class="section">
          <h3>💬 Room Chat</h3>
          <div class="input-group">
            <label>Room ID:</label>
            <input v-model.number="roomId" type="number" />
          </div>
          <div class="input-group">
            <label>Password (optional):</label>
            <input
              v-model="roomPassword"
              type="password"
              placeholder="Leave empty if no password"
            />
          </div>
          <button
            class="btn btn-primary"
            @click="joinRoom"
            :disabled="!isConnected || inRoom"
          >
            Join Room
          </button>
          <button class="btn btn-danger" @click="leaveRoom" :disabled="!inRoom">
            Leave Room
          </button>
        </div>

        <!-- Breakroom Chat -->
        <div class="section">
          <h3>🔸 Breakroom Chat</h3>
          <div class="input-group">
            <label>Breakroom ID:</label>
            <input v-model.number="breakroomId" type="number" />
          </div>
          <button
            class="btn btn-primary"
            @click="joinBreakroom"
            :disabled="!isConnected || inBreakroom"
          >
            Join Breakroom
          </button>
          <button
            class="btn btn-danger"
            @click="leaveBreakroom"
            :disabled="!inBreakroom"
          >
            Leave Breakroom
          </button>
        </div>

        <!-- P2P Chat -->
        <div class="section">
          <h3>👤 P2P Chat</h3>
          <div class="input-group">
            <label>Receiver User ID:</label>
            <input v-model.number="receiverId" type="number" />
          </div>
          <button
            class="btn btn-primary"
            @click="loadP2PHistory"
            :disabled="!isConnected || !receiverId"
          >
            Load History
          </button>
        </div>

        <!-- Chat Mode -->
        <div class="section">
          <h3>📡 Chat Mode</h3>
          <select v-model="chatMode" class="select-mode">
            <option value="room">Room Chat</option>
            <option value="breakroom">Breakroom Chat</option>
            <option value="p2p">P2P Chat</option>
          </select>
        </div>

        <!-- Logs -->
        <div class="section">
          <h3>📋 Event Logs</h3>
          <div class="logs" ref="logsContainer">
            <div v-for="(log, index) in logs" :key="index" class="log-entry">
              <span class="log-time">[{{ log.time }}]</span> {{ log.message }}
            </div>
          </div>
          <button class="btn btn-secondary" @click="clearLogs">
            Clear Logs
          </button>
        </div>
      </div>

      <!-- Chat Area -->
      <div class="chat-area">
        <div class="chat-header">
          <span>{{ chatTitle }}</span>
          <div v-if="paginationInfo.totalPages > 1" class="pagination-info">
            <button
              class="btn-pagination"
              @click="loadPreviousPage"
              :disabled="paginationInfo.currentPage <= 1"
            >
              ← Previous
            </button>
            <span class="page-info">
              Page {{ paginationInfo.currentPage }} /
              {{ paginationInfo.totalPages }} (Total:
              {{ paginationInfo.totalMessages }})
            </span>
            <button
              class="btn-pagination"
              @click="loadNextPage"
              :disabled="
                paginationInfo.currentPage >= paginationInfo.totalPages
              "
            >
              Next →
            </button>
          </div>
        </div>

        <div class="messages" ref="messagesContainer">
          <!-- Hidden file input for editing messages (outside v-for to avoid ref array issue) -->
          <input
            type="file"
            ref="editFileInput"
            accept="image/*"
            multiple
            style="display: none"
            @change="handleEditFileSelect"
          />

          <div
            v-for="(message, index) in messages"
            :key="index"
            :class="[
              'message',
              { own: isOwnMessage(message), system: message.type === 'system' },
            ]"
          >
            <template v-if="message.type !== 'system'">
              <div class="message-header">
                <span
                  ><strong
                    >User {{ message.userId || message.senderId }}</strong
                  ></span
                >
                <span>
                  {{ formatTime(message.createdDate) }}
                  <span v-if="message.isEdited" class="edited-badge"
                    >(edited)</span
                  >
                </span>
              </div>

              <!-- Edit mode -->
              <div
                v-if="editingMessageId === (message.id || message.messageId)"
                class="edit-message-section"
              >
                <textarea
                  v-model="editMessageText"
                  class="edit-message-input"
                  rows="3"
                  @keypress.ctrl.enter="saveEditMessage"
                ></textarea>

                <div v-if="editingImages.length > 0" class="image-preview">
                  <div
                    v-for="(img, index) in editingImages"
                    :key="index"
                    class="preview-item"
                  >
                    <img :src="img.url" alt="Preview" />
                    <button class="remove-btn" @click="removeEditFile(index)">
                      ✕
                    </button>
                  </div>
                </div>

                <div class="message-actions">
                  <button
                    class="btn-upload"
                    @click="$refs.editFileInput.click()"
                  >
                    📎 Add Images
                  </button>
                  <button class="btn-save" @click="saveEditMessage">
                    💾 Save
                  </button>
                  <button class="btn-cancel" @click="cancelEditMessage">
                    ❌ Cancel
                  </button>
                </div>
              </div>

              <!-- Normal mode -->
              <template v-else>
                <div class="message-content">{{ message.message }}</div>
                <div
                  v-if="message.imageUrls && message.imageUrls.length > 0"
                  class="message-images"
                >
                  <img
                    v-for="(url, idx) in message.imageUrls"
                    :key="idx"
                    :src="url"
                    alt="Image"
                    @click="openImage(url)"
                  />
                </div>

                <!-- Edit and Delete buttons for own messages -->
                <div v-if="isOwnMessage(message)" class="message-actions">
                  <button class="btn-edit" @click="startEditMessage(message)">
                    ✏️ Edit
                  </button>
                  <button class="btn-delete" @click="deleteMessage(message)">
                    🗑️ Delete
                  </button>
                </div>
              </template>
            </template>
            <template v-else>
              {{ message.text }}
            </template>
          </div>

          <div v-if="typingMessage" class="typing-indicator">
            {{ typingMessage }}
          </div>
        </div>

        <div class="chat-input">
          <input
            type="file"
            ref="fileInput"
            accept="image/*"
            multiple
            style="display: none"
            @change="handleFileSelect"
          />

          <div v-if="selectedFiles.length > 0" class="image-preview">
            <div
              v-for="(file, index) in selectedFiles"
              :key="index"
              class="preview-item"
            >
              <img :src="file.preview" alt="Preview" />
              <button class="remove-btn" @click="removeFile(index)">✕</button>
            </div>
          </div>

          <div class="input-row">
            <button class="btn-upload" @click="$refs.fileInput.click()">
              📎
            </button>
            <input
              v-model="newMessage"
              type="text"
              placeholder="Type a message..."
              @keypress.enter="sendMessage"
              @input="handleTyping"
            />
            <button class="btn-send" @click="sendMessage" :disabled="!canSend">
              Send
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted, onUnmounted, nextTick, watch } from "vue";
import { useRouter } from "vue-router";
import chatSignalRService from "@/services/chatSignalRService";
import axios from "axios";

export default {
  name: "ChatTest",
  setup() {
    const router = useRouter();

    // Auth
    const token = ref("");
    const userId = ref(null);
    const username = ref("");
    const apiUrl = ref("");

    // Connection
    const isConnected = ref(false);

    // Room state
    const roomId = ref(1);
    const roomPassword = ref("");
    const breakroomId = ref(1);
    const receiverId = ref(2);
    const inRoom = ref(false);
    const inBreakroom = ref(false);
    const chatMode = ref("room");

    // Messages
    const messages = ref([]);
    const newMessage = ref("");
    const selectedFiles = ref([]);
    const typingMessage = ref("");
    let typingTimeout = null;

    // Edit message
    const editingMessageId = ref(null);
    const editMessageText = ref("");
    const editingImages = ref([]);

    // Pagination
    const paginationInfo = ref({
      currentPage: 1,
      totalPages: 1,
      totalMessages: 0,
      pageSize: 50,
    });

    // Logs
    const logs = ref([]);
    const logsContainer = ref(null);
    const messagesContainer = ref(null);
    const fileInput = ref(null);
    const editFileInput = ref(null);

    // Computed
    const chatTitle = computed(() => {
      if (chatMode.value === "room" && inRoom.value) {
        return `Room #${roomId.value}`;
      } else if (chatMode.value === "breakroom" && inBreakroom.value) {
        return `Breakroom #${breakroomId.value}`;
      } else if (chatMode.value === "p2p") {
        return `P2P with User #${receiverId.value}`;
      }
      return "Select a room to start chatting";
    });

    const canSend = computed(() => {
      return (
        isConnected.value &&
        (newMessage.value.trim() || selectedFiles.value.length > 0)
      );
    });

    // Methods
    const addLog = (message) => {
      logs.value.push({
        time: new Date().toLocaleTimeString(),
        message,
      });
      nextTick(() => {
        if (logsContainer.value) {
          logsContainer.value.scrollTop = logsContainer.value.scrollHeight;
        }
      });
    };

    const clearLogs = () => {
      logs.value = [];
    };

    const addMessageToChat = (message, type = "message") => {
      messages.value.push({ ...message, type });
      scrollToBottom();
    };

    const addSystemMessage = (text) => {
      messages.value.push({ type: "system", text });
      scrollToBottom();
    };

    const scrollToBottom = () => {
      nextTick(() => {
        if (messagesContainer.value) {
          messagesContainer.value.scrollTop =
            messagesContainer.value.scrollHeight;
        }
      });
    };

    const isOwnMessage = (message) => {
      return (message.userId || message.senderId) == userId.value;
    };

    const formatTime = (date) => {
      return new Date(date).toLocaleTimeString("vi-VN", {
        hour: "2-digit",
        minute: "2-digit",
      });
    };

    const openImage = (url) => {
      window.open(url, "_blank");
    };

    // SignalR
    const connectSignalR = async () => {
      try {
        addLog("Connecting to SignalR...");
        const connected = await chatSignalRService.startConnection(
          token.value,
          apiUrl.value,
        );
        isConnected.value = connected;

        if (connected) {
          setupSignalREvents();
          addLog("✅ Connected to SignalR");
        }
      } catch (error) {
        addLog("❌ Connection failed: " + error.message);
      }
    };

    const setupSignalREvents = () => {
      chatSignalRService.onReceiveMessage((message) => {
        addLog("📩 Received message");
        addMessageToChat(message);
      });

      chatSignalRService.onReceiveP2PMessage((message) => {
        addLog("📩 Received P2P message");
        addMessageToChat(message);
      });

      chatSignalRService.onUserJoined((data) => {
        addLog(`👋 User ${data.userId} joined`);
        addSystemMessage(`User ${data.userId} joined`);
      });

      chatSignalRService.onUserLeft((data) => {
        addLog(`👋 User ${data.userId} left`);
        addSystemMessage(`User ${data.userId} left`);
      });

      chatSignalRService.onUserTyping((data) => {
        if (data.isTyping) {
          typingMessage.value = `User ${data.userId} is typing...`;
        } else {
          typingMessage.value = "";
        }
      });

      chatSignalRService.onUserTypingP2P((data) => {
        if (data.isTyping) {
          typingMessage.value = `User ${data.senderId} is typing...`;
        } else {
          typingMessage.value = "";
        }
      });

      chatSignalRService.onError((error) => {
        addLog("❌ Error: " + error);
        alert("Error: " + error);
      });

      chatSignalRService.onMessageEdited((data) => {
        console.log("📝 MessageEdited event received:", data);
        const messageId = data.Id || data.id;
        addLog(`✏️ Message ${messageId} edited`);

        // Find message index - handle both PascalCase (C#) and camelCase
        const messageIndex = messages.value.findIndex((m) => {
          const mId = m.id || m.messageId || m.Id;
          return mId === messageId;
        });

        if (messageIndex !== -1) {
          const oldMessage = messages.value[messageIndex];

          // Create updated message with normalized properties (both PascalCase and camelCase)
          const updatedMessage = {
            ...oldMessage,
            // Message content - both cases
            message: data.Message || data.message,
            Message: data.Message || data.message,
            // Edit flag - both cases
            isEdited: data.IsEdited !== undefined ? data.IsEdited : true,
            IsEdited: data.IsEdited !== undefined ? data.IsEdited : true,
            // Update date - both cases
            updatedDate:
              data.UpdatedDate || data.updatedDate || new Date().toISOString(),
            UpdatedDate:
              data.UpdatedDate || data.updatedDate || new Date().toISOString(),
            // Image URLs if provided - both cases
            imageUrls: data.ImageUrls || data.imageUrls || oldMessage.imageUrls,
            ImageUrls: data.ImageUrls || data.imageUrls || oldMessage.imageUrls,
          };

          // Use splice to trigger Vue reactivity
          messages.value.splice(messageIndex, 1, updatedMessage);
          console.log("✅ Message updated in UI:", updatedMessage);
        } else {
          console.warn("⚠️ Message not found in list, messageId:", messageId);
          console.log(
            "Available message IDs:",
            messages.value.map((m) => m.id || m.messageId || m.Id),
          );
        }
      });

      chatSignalRService.onMessageDeleted((data) => {
        console.log("🗑️ MessageDeleted event received:", data);
        const messageId = data.MessageId || data.messageId;
        addLog(`🗑️ Message ${messageId} deleted`);

        // Remove message from list - handle both PascalCase and camelCase
        const beforeCount = messages.value.length;
        messages.value = messages.value.filter((m) => {
          const mId = m.id || m.messageId || m.Id;
          return mId !== messageId;
        });
        const afterCount = messages.value.length;

        if (beforeCount > afterCount) {
          console.log(
            `✅ Message deleted from UI. Messages count: ${beforeCount} -> ${afterCount}`,
          );
        } else {
          console.warn(
            "⚠️ Message not found for deletion, messageId:",
            messageId,
          );
          console.log(
            "Available message IDs:",
            messages.value.map((m) => m.id || m.messageId || m.Id),
          );
        }
      });
    };

    // Room management
    const joinRoom = async () => {
      try {
        const password = roomPassword.value.trim() || null;
        await chatSignalRService.joinRoom(roomId.value, password);
        inRoom.value = true;
        addLog(`✅ Joined room ${roomId.value}`);
        await loadRoomHistory();
      } catch (error) {
        addLog(`❌ Join room failed: ${error.message}`);
        alert(`Join room failed: ${error.message}`);
      }
    };

    const leaveRoom = async () => {
      try {
        await chatSignalRService.leaveRoom(roomId.value);
        inRoom.value = false;
        addLog(`✅ Left room ${roomId.value}`);
      } catch (error) {
        addLog(`❌ Leave room failed: ${error.message}`);
      }
    };

    const joinBreakroom = async () => {
      try {
        await chatSignalRService.joinBreakroom(breakroomId.value);
        inBreakroom.value = true;
        addLog(`✅ Joined breakroom ${breakroomId.value}`);
        await loadBreakroomHistory();
      } catch (error) {
        addLog(`❌ Join breakroom failed: ${error.message}`);
      }
    };

    const leaveBreakroom = async () => {
      try {
        await chatSignalRService.leaveBreakroom(breakroomId.value);
        inBreakroom.value = false;
        addLog(`✅ Left breakroom ${breakroomId.value}`);
      } catch (error) {
        addLog(`❌ Leave breakroom failed: ${error.message}`);
      }
    };

    // Load history
    const loadRoomHistory = async () => {
      try {
        const response = await axios.get(
          `${apiUrl.value}/api/Chat/room/${roomId.value}/history?page=1&pageSize=50`,
          { headers: { Authorization: `Bearer ${token.value}` } },
        );
        if (response.data.isSuccess) {
          messages.value = [];
          response.data.data.messages.forEach((msg) => addMessageToChat(msg));
        }
      } catch (error) {
        addLog(`❌ Load history failed: ${error.message}`);
      }
    };

    const loadBreakroomHistory = async () => {
      try {
        const response = await axios.get(
          `${apiUrl.value}/api/Chat/breakroom/${breakroomId.value}/history?page=1&pageSize=50`,
          { headers: { Authorization: `Bearer ${token.value}` } },
        );
        if (response.data.isSuccess) {
          messages.value = [];
          response.data.data.messages.forEach((msg) => addMessageToChat(msg));
        }
      } catch (error) {
        addLog(`❌ Load history failed: ${error.message}`);
      }
    };

    const loadP2PHistory = async (page = 1, pageSize = 50) => {
      try {
        addLog(`📥 Loading P2P history with user ${receiverId.value}...`);
        const response = await axios.get(
          `${apiUrl.value}/api/Chat/p2p/${receiverId.value}/history?page=${page}&pageSize=${pageSize}`,
          { headers: { Authorization: `Bearer ${token.value}` } },
        );

        if (response.data.isSuccess) {
          messages.value = [];
          const data = response.data.data;

          // Update pagination info
          paginationInfo.value = {
            currentPage: data.currentPage,
            totalPages: data.totalPages,
            totalMessages: data.totalMessages,
            pageSize: data.pageSize,
          };

          addLog(
            `✅ Loaded ${data.messages.length} messages (Page ${data.currentPage}/${data.totalPages}, Total: ${data.totalMessages})`,
          );

          data.messages.forEach((msg) => addMessageToChat(msg));

          // Log pagination info
          if (data.totalPages > 1) {
            addSystemMessage(
              `📄 Page ${data.currentPage} of ${data.totalPages} | Total messages: ${data.totalMessages}`,
            );
          }
        }
      } catch (error) {
        addLog(`❌ Load P2P history failed: ${error.message}`);
        alert(`Load history failed: ${error.message}`);
      }
    };

    const loadPreviousPage = () => {
      if (paginationInfo.value.currentPage > 1) {
        if (chatMode.value === "p2p") {
          loadP2PHistory(
            paginationInfo.value.currentPage - 1,
            paginationInfo.value.pageSize,
          );
        }
      }
    };

    const loadNextPage = () => {
      if (paginationInfo.value.currentPage < paginationInfo.value.totalPages) {
        if (chatMode.value === "p2p") {
          loadP2PHistory(
            paginationInfo.value.currentPage + 1,
            paginationInfo.value.pageSize,
          );
        }
      }
    };

    // Send message
    const sendMessage = async () => {
      if (!canSend.value) return;

      try {
        let imageUrls = null;

        if (selectedFiles.value.length > 0) {
          imageUrls = await uploadImages();
          if (!imageUrls) {
            alert("Upload images failed");
            return;
          }
        }

        if (chatMode.value === "room") {
          if (!inRoom.value) {
            alert("Please join a room first");
            return;
          }
          await chatSignalRService.sendMessageToRoom(
            roomId.value,
            newMessage.value,
            imageUrls,
          );
        } else if (chatMode.value === "breakroom") {
          if (!inBreakroom.value) {
            alert("Please join a breakroom first");
            return;
          }
          await chatSignalRService.sendMessageToBreakroom(
            breakroomId.value,
            newMessage.value,
            imageUrls,
          );
        } else if (chatMode.value === "p2p") {
          await chatSignalRService.sendMessageP2P(
            receiverId.value,
            newMessage.value,
            imageUrls,
          );
        }

        newMessage.value = "";
        selectedFiles.value = [];
        addLog("✅ Message sent");
      } catch (error) {
        addLog(`❌ Send message failed: ${error.message}`);
        alert("Send failed: " + error.message);
      }
    };

    // Upload images
    const uploadImages = async () => {
      try {
        const formData = new FormData();
        selectedFiles.value.forEach((file) => {
          formData.append("images", file.file);
        });

        const response = await axios.post(
          `${apiUrl.value}/api/Chat/upload-images`,
          formData,
          { headers: { Authorization: `Bearer ${token.value}` } },
        );

        if (response.data.isSuccess) {
          return response.data.data.uploadedUrls;
        }
        return null;
      } catch (error) {
        addLog(`❌ Upload failed: ${error.message}`);
        return null;
      }
    };

    // File handling
    const handleFileSelect = (event) => {
      const files = Array.from(event.target.files);
      files.forEach((file) => {
        const reader = new FileReader();
        reader.onload = (e) => {
          selectedFiles.value.push({
            file: file,
            preview: e.target.result,
          });
        };
        reader.readAsDataURL(file);
      });
    };

    const removeFile = (index) => {
      selectedFiles.value.splice(index, 1);
    };

    // Edit and Delete message
    const startEditMessage = (message) => {
      console.log("📝 Starting edit message:", message);
      const messageId = message.id || message.messageId;
      console.log("Message ID:", messageId);
      console.log("Message content:", message.message);
      console.log("Current chat mode:", chatMode.value);

      editingMessageId.value = messageId;
      editMessageText.value = message.message;
      // Convert existing image URLs to new format {url, file: null, isExisting: true}
      editingImages.value = (message.imageUrls || []).map((url) => ({
        url,
        file: null,
        isExisting: true,
      }));

      console.log("Edit state set - editingMessageId:", editingMessageId.value);
      console.log("Edit state set - editMessageText:", editMessageText.value);
      console.log("Edit state set - editingImages:", editingImages.value);
    };

    const cancelEditMessage = () => {
      editingMessageId.value = null;
      editMessageText.value = "";
      editingImages.value = [];
    };

    const handleEditFileSelect = async (event) => {
      const files = Array.from(event.target.files);
      for (const file of files) {
        const reader = new FileReader();
        reader.onload = (e) => {
          editingImages.value.push({
            url: e.target.result,
            file: file,
            isExisting: false,
          });
        };
        reader.readAsDataURL(file);
      }
      // Reset input to allow selecting same file again
      event.target.value = "";
    };

    const removeEditFile = (index) => {
      editingImages.value.splice(index, 1);
    };

    const uploadEditImages = async () => {
      try {
        // Separate existing URLs and new files
        const existingUrls = editingImages.value
          .filter((img) => img.isExisting)
          .map((img) => img.url);

        const newFiles = editingImages.value.filter(
          (img) => !img.isExisting && img.file,
        );

        // If no new files, return existing URLs
        if (newFiles.length === 0) {
          return existingUrls.length > 0 ? existingUrls : null;
        }

        // Upload new files with proper filenames
        const formData = new FormData();
        for (const item of newFiles) {
          // Use actual file object which preserves filename and mimetype
          formData.append("images", item.file, item.file.name);
        }

        console.log("📤 Uploading", newFiles.length, "new image(s)...");
        const response = await axios.post(
          `${apiUrl.value}/api/Chat/upload-images`,
          formData,
          {
            headers: {
              Authorization: `Bearer ${token.value}`,
              "Content-Type": "multipart/form-data",
            },
          },
        );

        if (response.data.isSuccess) {
          const uploadedUrls = response.data.data.uploadedUrls;
          console.log("✅ Uploaded URLs:", uploadedUrls);
          // Combine existing URLs with newly uploaded URLs
          const allUrls = [...existingUrls, ...uploadedUrls];
          console.log("📋 Final image URLs:", allUrls);
          return allUrls.length > 0 ? allUrls : null;
        }
        return null;
      } catch (error) {
        console.error("❌ Upload error:", error);
        addLog(`❌ Upload failed: ${error.message}`);
        return null;
      }
    };

    const saveEditMessage = async () => {
      console.log("💾 saveEditMessage called");
      console.log("editMessageText:", editMessageText.value);
      console.log("editingMessageId:", editingMessageId.value);
      console.log("chatMode:", chatMode.value);

      if (!editMessageText.value.trim() || !editingMessageId.value) {
        console.warn("⚠️ Validation failed - empty text or no message ID");
        return;
      }

      const messageId = editingMessageId.value;
      console.log("🔑 Editing message ID:", messageId);

      try {
        // Upload images - uploadEditImages returns null if no images, or array of URLs
        let imageUrls = null;
        if (editingImages.value.length > 0) {
          imageUrls = await uploadEditImages();
          // Only return if upload explicitly failed (null when there were files to upload)
          if (
            imageUrls === null &&
            editingImages.value.some((img) => !img.isExisting)
          ) {
            addLog("❌ Failed to upload images");
            return;
          }
        }

        console.log("🖼️ Final imageUrls to send:", imageUrls);

        if (chatMode.value === "room") {
          console.log("🟢 Calling editMessageInRoom...");
          console.log("Parameters:", {
            messageId,
            roomId: roomId.value,
            newMessage: editMessageText.value,
            imageUrls,
          });

          await chatSignalRService.editMessageInRoom(
            messageId,
            roomId.value,
            editMessageText.value,
            imageUrls,
          );

          console.log("✅ editMessageInRoom completed");
        } else if (chatMode.value === "breakroom") {
          console.log("🔵 Calling editMessageInBreakroom...");
          console.log("Parameters:", {
            messageId,
            breakroomId: breakroomId.value,
            newMessage: editMessageText.value,
            imageUrls,
          });

          await chatSignalRService.editMessageInBreakroom(
            messageId,
            breakroomId.value,
            editMessageText.value,
            imageUrls,
          );

          console.log("✅ editMessageInBreakroom completed");
        } else if (chatMode.value === "p2p") {
          console.log("🔴 Calling editMessageP2P...");
          console.log("Parameters:", {
            messageId,
            newMessage: editMessageText.value,
            imageUrls,
          });

          await chatSignalRService.editMessageP2P(
            messageId,
            editMessageText.value,
            imageUrls,
          );

          console.log("✅ editMessageP2P completed");
        }

        editingMessageId.value = null;
        editMessageText.value = "";
        editingImages.value = [];
        addLog("✅ Message edited");
        console.log("✅ Edit completed successfully");
      } catch (error) {
        console.error("❌ Edit message error:", error);
        console.error("Error message:", error.message);
        console.error("Error stack:", error.stack);
        console.error("Error details:", JSON.stringify(error, null, 2));

        addLog(`❌ Edit message failed: ${error.message}`);
        alert("Edit message failed: " + error.message);
      }
    };

    const deleteMessage = async (message) => {
      console.log("🗑️ deleteMessage called:", message);

      if (!confirm("Are you sure you want to delete this message?")) {
        console.log("❌ Delete cancelled by user");
        return;
      }

      const messageId = message.id || message.messageId;
      console.log("🔑 Deleting message ID:", messageId);
      console.log("chatMode:", chatMode.value);

      try {
        if (chatMode.value === "room") {
          console.log("🟢 Calling deleteMessageInRoom...");
          await chatSignalRService.deleteMessageInRoom(messageId, roomId.value);
        } else if (chatMode.value === "breakroom") {
          console.log("🔵 Calling deleteMessageInBreakroom...");
          await chatSignalRService.deleteMessageInBreakroom(
            messageId,
            breakroomId.value,
          );
        } else if (chatMode.value === "p2p") {
          console.log("🔴 Calling deleteMessageP2P...");
          console.log("Parameters:", {
            messageId,
            receiverId: receiverId.value,
          });
          await chatSignalRService.deleteMessageP2P(
            messageId,
            receiverId.value,
          );
        }

        console.log("✅ Delete completed successfully");
        addLog("✅ Message deleted");
      } catch (error) {
        console.error("❌ Delete message error:", error);
        console.error("Error message:", error.message);
        console.error("Error stack:", error.stack);
        console.error("Error details:", JSON.stringify(error, null, 2));

        addLog(`❌ Delete message failed: ${error.message}`);
        alert("Delete message failed: " + error.message);
      }
    };

    // Typing indicator
    const handleTyping = () => {
      if (chatMode.value === "room" && inRoom.value) {
        chatSignalRService.startTypingInRoom(roomId.value);
        clearTimeout(typingTimeout);
        typingTimeout = setTimeout(() => {
          chatSignalRService.stopTypingInRoom(roomId.value);
        }, 1000);
      } else if (chatMode.value === "p2p") {
        chatSignalRService.startTypingP2P(receiverId.value);
        clearTimeout(typingTimeout);
        typingTimeout = setTimeout(() => {
          chatSignalRService.stopTypingP2P(receiverId.value);
        }, 1000);
      }
    };

    // Logout
    const logout = () => {
      localStorage.clear();
      router.push("/login");
    };

    // Lifecycle
    onMounted(async () => {
      token.value = localStorage.getItem("convox_token");
      userId.value = localStorage.getItem("convox_userId");
      username.value = localStorage.getItem("convox_username");
      apiUrl.value = localStorage.getItem("convox_apiUrl");

      if (!token.value || !userId.value) {
        alert("Please login first");
        router.push("/login");
        return;
      }

      await connectSignalR();
    });

    onUnmounted(async () => {
      clearTimeout(typingTimeout);
      if (inRoom.value) await leaveRoom();
      if (inBreakroom.value) await leaveBreakroom();
      await chatSignalRService.disconnect();
    });

    // Watchers - Auto load history when chat mode or receiver changes
    watch(
      [chatMode, receiverId],
      ([newMode, newReceiverId], [oldMode, oldReceiverId]) => {
        // Clear messages when switching modes
        messages.value = [];
        paginationInfo.value = {
          currentPage: 1,
          totalPages: 1,
          totalMessages: 0,
          pageSize: 50,
        };

        // Auto load history for P2P mode
        if (newMode === "p2p" && newReceiverId && isConnected.value) {
          // Only load if receiverId changed or just switched to p2p mode
          if (oldMode !== "p2p" || newReceiverId !== oldReceiverId) {
            addLog(`🔄 Auto-loading P2P history with user ${newReceiverId}...`);
            loadP2PHistory();
          }
        }
      },
    );

    return {
      // Auth
      userId,
      username,

      // Connection
      isConnected,

      // Room state
      roomId,
      roomPassword,
      breakroomId,
      receiverId,
      inRoom,
      inBreakroom,
      chatMode,
      chatTitle,

      // Messages
      messages,
      newMessage,
      selectedFiles,
      typingMessage,
      canSend,

      // Pagination
      paginationInfo,

      // Logs
      logs,
      logsContainer,
      messagesContainer,
      fileInput,
      editFileInput,

      // Methods
      joinRoom,
      leaveRoom,
      joinBreakroom,
      leaveBreakroom,
      loadP2PHistory,
      loadPreviousPage,
      loadNextPage,
      sendMessage,
      handleFileSelect,
      removeFile,
      handleTyping,
      clearLogs,
      logout,
      isOwnMessage,
      formatTime,
      openImage,

      // Edit message
      editingMessageId,
      editMessageText,
      editingImages,
      startEditMessage,
      cancelEditMessage,
      saveEditMessage,
      deleteMessage,
      handleEditFileSelect,
      removeEditFile,
    };
  },
};
</script>

<style scoped>
.chat-test {
  min-height: 100vh;
  background: #f0f2f5;
  padding: 20px;
}

.header {
  background: white;
  padding: 15px 20px;
  border-radius: 8px;
  margin-bottom: 20px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.user-info {
  display: flex;
  align-items: center;
  gap: 15px;
}

.header-actions {
  display: flex;
  gap: 15px;
  align-items: center;
}

.status {
  display: flex;
  align-items: center;
  gap: 5px;
}

.status-dot {
  width: 10px;
  height: 10px;
  border-radius: 50%;
  background: #ccc;
}

.status-dot.connected {
  background: #4caf50;
  animation: pulse 2s infinite;
}

@keyframes pulse {
  0%,
  100% {
    opacity: 1;
  }
  50% {
    opacity: 0.5;
  }
}

.btn-logout {
  padding: 8px 16px;
  background: #f44336;
  color: white;
  border: none;
  border-radius: 5px;
  cursor: pointer;
}

.container {
  display: grid;
  grid-template-columns: 300px 1fr;
  gap: 20px;
  height: calc(100vh - 120px);
}

.sidebar {
  background: white;
  border-radius: 8px;
  padding: 20px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  overflow-y: auto;
}

.section {
  margin-bottom: 30px;
}

.section h3 {
  margin-bottom: 15px;
  color: #333;
  font-size: 16px;
}

.input-group {
  margin-bottom: 10px;
}

.input-group label {
  display: block;
  margin-bottom: 5px;
  font-size: 13px;
  color: #666;
}

.input-group input {
  width: 100%;
  padding: 8px;
  border: 1px solid #ddd;
  border-radius: 4px;
}

.select-mode {
  width: 100%;
  padding: 8px;
  border: 1px solid #ddd;
  border-radius: 4px;
}

.btn {
  width: 100%;
  padding: 10px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  font-size: 14px;
  font-weight: 600;
  margin-top: 10px;
}

.btn-primary {
  background: #4caf50;
  color: white;
}

.btn-secondary {
  background: #2196f3;
  color: white;
}

.btn-danger {
  background: #f44336;
  color: white;
}

.btn:disabled {
  background: #ccc;
  cursor: not-allowed;
}

.logs {
  background: #263238;
  color: #aed581;
  padding: 15px;
  border-radius: 5px;
  font-family: "Courier New", monospace;
  font-size: 12px;
  max-height: 200px;
  overflow-y: auto;
}

.log-entry {
  margin-bottom: 5px;
}

.log-time {
  color: #78909c;
}

.chat-area {
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  display: flex;
  flex-direction: column;
}

.chat-header {
  padding: 15px 20px;
  border-bottom: 1px solid #ddd;
  font-weight: 600;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.pagination-info {
  display: flex;
  align-items: center;
  gap: 10px;
  font-size: 13px;
}

.btn-pagination {
  padding: 5px 12px;
  background: #2196f3;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 12px;
  transition: background 0.2s;
}

.btn-pagination:hover:not(:disabled) {
  background: #1976d2;
}

.btn-pagination:disabled {
  background: #ccc;
  cursor: not-allowed;
}

.page-info {
  color: #666;
  font-weight: normal;
  white-space: nowrap;
}

.messages {
  flex: 1;
  overflow-y: auto;
  padding: 20px;
  background: #f9f9f9;
}

.message {
  margin-bottom: 15px;
  padding: 10px 15px;
  background: white;
  border-radius: 8px;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.1);
}

.message.own {
  background: #dcf8c6;
  margin-left: auto;
  max-width: 70%;
}

.message.system {
  background: #fff3cd;
  text-align: center;
  font-style: italic;
}

.message-header {
  display: flex;
  justify-content: space-between;
  margin-bottom: 5px;
  font-size: 12px;
  color: #666;
}

.message-content {
  color: #333;
  word-wrap: break-word;
}

.message-images {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
  margin-top: 10px;
}

.message-images img {
  max-width: 200px;
  border-radius: 5px;
  cursor: pointer;
}

.typing-indicator {
  padding: 10px;
  color: #666;
  font-style: italic;
}

.chat-input {
  border-top: 1px solid #ddd;
  padding: 15px 20px;
}

.image-preview {
  display: flex;
  gap: 10px;
  margin-bottom: 10px;
  flex-wrap: wrap;
}

.preview-item {
  position: relative;
}

.preview-item img {
  width: 80px;
  height: 80px;
  object-fit: cover;
  border-radius: 5px;
}

.remove-btn {
  position: absolute;
  top: -5px;
  right: -5px;
  background: #f44336;
  color: white;
  border: none;
  border-radius: 50%;
  width: 20px;
  height: 20px;
  cursor: pointer;
}

.input-row {
  display: flex;
  gap: 10px;
}

.input-row input {
  flex: 1;
  padding: 12px;
  border: 1px solid #ddd;
  border-radius: 5px;
}

.btn-upload {
  padding: 12px 20px;
  background: #9e9e9e;
  color: white;
  border: none;
  border-radius: 5px;
  cursor: pointer;
}

.btn-send {
  padding: 12px 20px;
  background: #4caf50;
  color: white;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  font-weight: 600;
}

.btn-send:disabled {
  background: #ccc;
  cursor: not-allowed;
}

/* Edit Message Styling */
.edited-badge {
  color: #ff9800;
  font-style: italic;
  font-size: 10px;
  margin-left: 5px;
}

.edit-message-section {
  margin-top: 8px;
}

.edit-message-input {
  width: 100%;
  padding: 8px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
  font-family: inherit;
  resize: vertical;
  margin-bottom: 8px;
}

.message-actions {
  display: flex;
  gap: 8px;
  margin-top: 8px;
}

.btn-edit,
.btn-delete,
.btn-save,
.btn-cancel {
  padding: 4px 12px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 12px;
  transition: all 0.2s;
}

.btn-edit {
  background: #2196f3;
  color: white;
}

.btn-edit:hover {
  background: #1976d2;
}

.btn-delete {
  background: #f44336;
  color: white;
}

.btn-delete:hover {
  background: #d32f2f;
}

.btn-save {
  background: #4caf50;
  color: white;
}

.btn-save:hover {
  background: #388e3c;
}

.btn-cancel {
  background: #9e9e9e;
  color: white;
}

.btn-cancel:hover {
  background: #757575;
}
</style>
