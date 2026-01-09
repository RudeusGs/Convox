<template>
  <div class="home-root">
    <div v-if="!activeServer" class="home-empty">
      <div class="home-empty-card">
        <h4>Chào mừng đến Convox</h4>
        <p>Chọn một máy chủ ở cột bên trái để bắt đầu.</p>
      </div>
    </div>

    <template v-else>
      <div v-if="isHub" class="home-hub">
        <header class="home-header">
          <div class="header-main">
            <div class="header-title-row">
              <div class="header-icon">
                <span>{{ activeServer.name?.charAt(0)?.toUpperCase() }}</span>
              </div>
              <div class="header-text">
                <div class="header-title">
                  Lớp học chung · {{ activeServer.name }}
                </div>
                <div class="header-subtitle">
                  Nơi giáo viên và cả lớp cùng trao đổi chung, điểm danh, thông báo.
                </div>
              </div>
            </div>
          </div>
        </header>

        <div class="home-body hub-layout">
          <section class="hub-left">
            <div class="hub-block">
              <div class="hub-block-title-row">
                <h5>Người tham gia</h5>
                <span class="hub-pill">{{ participants.length }} thành viên</span>
              </div>
              <ul class="participant-list">
                <li
                  v-for="m in participants"
                  :key="m.id"
                  class="participant-item"
                >
                  <div class="participant-avatar">
                    <span>{{ m.name.charAt(0).toUpperCase() }}</span>
                  </div>
                  <div class="participant-main">
                    <div class="participant-name">
                      {{ m.name }}
                      <span v-if="m.role === 'teacher'" class="participant-tag">
                        Giáo viên
                      </span>
                    </div>
                    <div class="participant-meta">
                      {{ m.note }}
                    </div>
                  </div>
                </li>
              </ul>
            </div>
          </section>

          <section class="hub-right">
            <div class="hub-block">
              <h5>Kênh trong lớp</h5>
              <div class="hub-chips">
                <div class="hub-chip">
                  <div class="chip-title">Chat chung</div>
                  <div class="chip-desc">Nhắn tin, hỏi bài, gửi thông báo nhanh.</div>
                </div>
                <div class="hub-chip">
                  <div class="chip-title">Phòng học nhóm</div>
                  <div class="chip-desc">Tự tạo phòng nhỏ để làm bài cùng nhau.</div>
                </div>
                <div class="hub-chip">
                  <div class="chip-title">Voice lớp</div>
                  <div class="chip-desc">Giáo viên mở mic, cả lớp nghe và đặt câu hỏi.</div>
                </div>
              </div>
            </div>

            <div class="hub-block">
              <h5>Mẹo nhỏ</h5>
              <ul class="hub-tip-list">
                <li>Đổi tên hiển thị giống tên thật để giáo viên dễ điểm danh.</li>
                <li>Dùng kênh voice khi thảo luận nhóm để đỡ spam chat.</li>
                <li>Tạo phòng riêng cho từng nhóm bài tập để khỏi lẫn lộn.</li>
              </ul>
            </div>
          </section>
        </div>
      </div>

      <div v-else-if="isTextChannel" class="home-chat">
        <header class="home-header">
          <div class="header-main">
            <div class="header-title-row">
              <div class="header-hash">
                #
              </div>
              <div class="header-text">
                <div class="header-title">
                  {{ activeChannel.name }}
                </div>
                <div class="header-subtitle">
                  {{ activeChannel.topic || 'Kênh chat của lớp học.' }}
                </div>
              </div>
            </div>
          </div>
        </header>

        <div class="home-body chat-layout">
          <div class="chat-messages">
            <div
              v-for="msg in messages"
              :key="msg.id"
              class="msg-row"
            >
              <div class="msg-avatar">
                <span>{{ msg.author.charAt(0).toUpperCase() }}</span>
              </div>
              <div class="msg-main">
                <div class="msg-head">
                  <span class="msg-author">{{ msg.author }}</span>
                  <span class="msg-time">{{ msg.time }}</span>
                </div>
                <div class="msg-text">
                  {{ msg.text }}
                </div>
              </div>
            </div>
          </div>

          <div class="chat-input-row">
            <div class="chat-input-wrap">
              <input
                v-model="draft"
                type="text"
                class="chat-input"
                :placeholder="`Nhắn gì đó trong #${activeChannel.name}...`"
                @keyup.enter="sendMessage"
              />
              <div class="chat-input-actions">
                <button type="button" class="icon-btn-sm">
                  <i class="bi bi-paperclip"></i>
                </button>
                <button type="button" class="icon-btn-sm">
                  <i class="bi bi-emoji-smile"></i>
                </button>
                <button type="button" class="icon-btn-sm send-btn" @click="sendMessage">
                  <i class="bi bi-send-fill"></i>
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div v-else-if="isVoiceChannel" class="home-voice">
        <header class="home-header">
          <div class="header-main">
            <div class="header-title-row">
              <div class="header-voice-icon">
                <i class="bi bi-broadcast-pin"></i>
              </div>
              <div class="header-text">
                <div class="header-title">
                  {{ activeChannel.name }}
                </div>
                <div class="header-subtitle">
                  Kênh thoại · bật mic, nói chuyện như trong lớp.
                </div>
              </div>
            </div>
          </div>
        </header>

        <div class="home-body voice-layout">
          <section class="voice-left">
            <div class="voice-card">
              <div class="voice-card-head">
                <h5>Đang trong phòng</h5>
                <span class="hub-pill">{{ voiceMembers.length }} người</span>
              </div>
              <ul class="voice-member-list">
                <li
                  v-for="m in voiceMembers"
                  :key="m.id"
                  class="voice-member-item"
                >
                  <div class="voice-avatar">
                    <span>{{ m.name.charAt(0).toUpperCase() }}</span>
                  </div>
                  <div class="voice-main">
                    <div class="voice-name">
                      {{ m.name }}
                      <span v-if="m.isSpeaking" class="voice-speaking-dot"></span>
                    </div>
                    <div class="voice-meta">
                      {{ m.status }}
                    </div>
                  </div>
                  <div class="voice-icons">
                    <i
                      class="bi"
                      :class="m.muted ? 'bi-mic-mute-fill' : 'bi-mic-fill'"
                    ></i>
                    <i class="bi bi-headphones"></i>
                  </div>
                </li>
              </ul>
            </div>
          </section>

          <section class="voice-right">
            <div v-if="screenShareActive" class="screen-share-card">
              <div class="screen-share-header">
                <div>
                  <div class="screen-share-title">Đang chia sẻ màn hình</div>
                  <div class="screen-share-meta">
                    {{ screenShareOwner.name }} • {{ screenShareTitle }}
                  </div>
                </div>
                <span class="screen-share-pill">Màn hình</span>
              </div>

              <div class="screen-share-preview">
                <div class="screen-share-sim">
                  <div class="screen-toolbar">
                    <span class="dot red"></span>
                    <span class="dot yellow"></span>
                    <span class="dot green"></span>
                    <span class="screen-toolbar-title">{{ screenShareTitle }}</span>
                  </div>
                  <div class="screen-content">
                    <div class="screen-block main"></div>
                    <div class="screen-block side"></div>
                  </div>
                </div>
              </div>

              <div class="screen-share-actions">
                <button type="button" class="screen-btn" @click="toggleFullscreen">
                  <i class="bi bi-arrows-fullscreen"></i>
                  <span>Phóng to</span>
                </button>
                <button type="button" class="screen-btn" @click="hideScreen">
                  <i class="bi bi-eye-slash"></i>
                  <span>Ẩn khung</span>
                </button>
                <button type="button" class="screen-btn danger" @click="leaveScreen">
                  <i class="bi bi-x-circle"></i>
                  <span>Rời xem</span>
                </button>
              </div>
            </div>

            <div class="voice-status-card">
              <div class="voice-wave"></div>
              <div class="voice-status-text">
                <div class="voice-status-title">
                  {{ screenShareActive ? 'Bạn đang ở trong kênh thoại' : 'Chờ giáo viên chia sẻ màn hình' }}
                </div>
                <div class="voice-status-sub">
                  Dùng các nút ở góc trái dưới để tắt/mở mic, loa, camera.
                </div>
                <button
                  v-if="!screenShareActive"
                  type="button"
                  class="screen-request-btn"
                  @click="requestScreenShare"
                >
                  <i class="bi bi-display"></i>
                  <span>Yêu cầu chia sẻ màn hình</span>
                </button>
              </div>
            </div>
          </section>
        </div>
      </div>

      <div v-else class="home-empty">
        <div class="home-empty-card">
          <h4>Chọn một kênh</h4>
          <p>Hãy chọn kênh chat hoặc kênh voice ở sidebar để bắt đầu.</p>
        </div>
      </div>
    </template>
  </div>
</template>

<script>
export default {
  name: 'HomeView',
  props: {
    activeChannel: {
      type: Object,
      default: null,
    },
    activeServer: {
      type: Object,
      default: null,
    },
    activeDM: {
      type: Object,
      default: null,
    },
  },
  data() {
    return {
      draft: '',
      messages: [
        {
          id: 1,
          author: 'Giáo viên',
          time: '09:00',
          text: 'Chào cả lớp, hôm nay mình ôn lại bài cũ rồi làm quiz nhanh nhé.',
        },
        {
          id: 2,
          author: 'Bạn',
          time: '09:01',
          text: 'Thầy/cô share slide với ạ 🙌',
        },
      ],
      screenShareActive: true,
      screenShareOwner: {
        name: 'Cô Mai',
      },
      screenShareTitle: 'Slide ôn tập chương 1',
    }
  },
  computed: {
    isHub() {
      if (!this.activeServer) return false
      if (!this.activeChannel) return true
      return this.activeChannel.type === 'hub'
    },
    isTextChannel() {
      return this.activeChannel && this.activeChannel.type === 'text'
    },
    isVoiceChannel() {
      return this.activeChannel && this.activeChannel.type === 'voice'
    },
    participants() {
      return [
        {
          id: 't1',
          name: 'Cô Mai',
          role: 'teacher',
          note: 'Giáo viên chủ nhiệm',
        },
        {
          id: 's1',
          name: 'Ngọc',
          role: 'student',
          note: 'Thường hỏi bài toán',
        },
        {
          id: 's2',
          name: 'Quân',
          role: 'student',
          note: 'Chuyên trực voice lớp',
        },
        {
          id: 's3',
          name: 'Minh',
          role: 'student',
          note: 'Team design slide',
        },
      ]
    },
    voiceMembers() {
      return [
        {
          id: 't1',
          name: 'Cô Mai',
          muted: false,
          isSpeaking: true,
          status: this.screenShareActive ? 'Đang giảng và chia sẻ màn hình' : 'Đang giảng bài',
        },
        {
          id: 's1',
          name: 'Ngọc',
          muted: true,
          isSpeaking: false,
          status: 'Đang nghe',
        },
        {
          id: 's2',
          name: 'Quân',
          muted: false,
          isSpeaking: false,
          status: 'Chuẩn bị hỏi',
        },
      ]
    },
  },
  methods: {
    sendMessage() {
      const text = this.draft.trim()
      if (!text) return
      this.messages.push({
        id: Date.now(),
        author: 'Bạn',
        time: new Date().toLocaleTimeString('vi-VN', {
          hour: '2-digit',
          minute: '2-digit',
        }),
        text,
      })
      this.draft = ''
      this.$nextTick(() => {
        const container = this.$el.querySelector('.chat-messages')
        if (container) {
          container.scrollTop = container.scrollHeight
        }
      })
    },
    requestScreenShare() {
      console.log('Yêu cầu chia sẻ màn hình được gửi')
    },
    toggleFullscreen() {
      console.log('Phóng to màn hình chia sẻ (placeholder)')
    },
    hideScreen() {
      this.screenShareActive = false
    },
    leaveScreen() {
      this.screenShareActive = false
      console.log('Rời xem màn hình chia sẻ')
    },
  },
}
</script>

<style scoped>
.home-root {
  display: flex;
  flex-direction: column;
  flex: 1;
  min-width: 0;
  min-height: 0;
  width: 100%;
  height: 100%;
  background-color: #313338;
  color: #e5e7eb;
}

.home-header {
  flex-shrink: 0;
  height: 56px;
  border-bottom: 1px solid #202225;
  padding: 8px 16px;
  display: flex;
  align-items: center;
}

.header-main {
  flex: 1;
  min-width: 0;
}

.header-title-row {
  display: flex;
  align-items: center;
  gap: 10px;
}

.header-icon {
  width: 32px;
  height: 32px;
  border-radius: 12px;
  background-color: #1e1f22;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 600;
  font-size: 15px;
  color: #e5e7eb;
}

.header-hash {
  width: 26px;
  height: 26px;
  border-radius: 999px;
  background-color: #1e1f22;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  font-size: 16px;
  color: #9ca3af;
}

.header-voice-icon {
  width: 30px;
  height: 30px;
  border-radius: 999px;
  background-color: #1e1f22;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #38bdf8;
  font-size: 18px;
}

.header-text {
  min-width: 0;
}

.header-title {
  font-size: 15px;
  font-weight: 600;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.header-subtitle {
  font-size: 12px;
  color: #9ca3af;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.home-body {
  flex: 1;
  min-height: 0;
  display: flex;
  padding: 12px 14px;
  gap: 12px;
  overflow: hidden;
}

.home-empty {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
}

.home-empty-card {
  text-align: center;
  padding: 16px 20px;
  border-radius: 14px;
  background-color: #2b2d31;
  border: 1px solid #202225;
}

/* HUB */

.hub-layout {
  width: 100%;
  display: grid;
  grid-template-columns: minmax(0, 1.2fr) minmax(0, 1fr);
  gap: 12px;
}

.hub-left,
.hub-right {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.hub-block {
  background-color: #2b2d31;
  border-radius: 12px;
  padding: 10px 12px;
  border: 1px solid #202225;
  min-height: 0;
}

.hub-block-title-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 6px;
}

.hub-block h5 {
  font-size: 14px;
  margin: 0;
}

.hub-pill {
  font-size: 11px;
  padding: 3px 8px;
  border-radius: 999px;
  background-color: #1e1f22;
  color: #e5e7eb;
}

/* participants */

.participant-list {
  list-style: none;
  padding: 0;
  margin: 0;
  display: flex;
  flex-direction: column;
  gap: 6px;
  overflow-y: auto;
}

.participant-item {
  display: flex;
  align-items: center;
  gap: 8px;
}

.participant-avatar {
  width: 30px;
  height: 30px;
  border-radius: 999px;
  background-color: #1e1f22;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 14px;
  font-weight: 600;
  flex-shrink: 0;
}

.participant-main {
  flex: 1;
  min-width: 0;
}

.participant-name {
  font-size: 13px;
  font-weight: 500;
  display: flex;
  align-items: center;
  gap: 6px;
}

.participant-tag {
  font-size: 10px;
  padding: 1px 6px;
  border-radius: 999px;
  background-color: #5865f2;
  color: #ffffff;
}

.participant-meta {
  font-size: 11px;
  color: #9ca3af;
}

/* hub chips */

.hub-chips {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.hub-chip {
  border-radius: 10px;
  background-color: #1e1f22;
  padding: 6px 8px;
}

.chip-title {
  font-size: 13px;
  font-weight: 500;
}

.chip-desc {
  font-size: 11px;
  color: #9ca3af;
  margin-top: 2px;
}

.hub-tip-list {
  list-style: none;
  padding-left: 0;
  margin: 0;
  font-size: 12px;
  color: #d1d5db;
}

.hub-tip-list li + li {
  margin-top: 4px;
}

/* CHAT */

.home-chat {
  display: flex;
  flex-direction: column;
  flex: 1;
  min-width: 0;
  min-height: 0;
}

.chat-layout {
  flex-direction: column;
  width: 100%;
}

.chat-messages {
  flex: 1;
  min-height: 0;
  background-color: #313338;
  border-radius: 10px;
  padding: 10px 10px 6px;
  overflow-y: auto;
  border: 1px solid #202225;
}

.msg-row {
  display: flex;
  align-items: flex-start;
  gap: 8px;
  margin-bottom: 8px;
}

.msg-avatar {
  width: 32px;
  height: 32px;
  border-radius: 999px;
  background-color: #1e1f22;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 14px;
  font-weight: 600;
  flex-shrink: 0;
}

.msg-main {
  flex: 1;
  min-width: 0;
}

.msg-head {
  display: flex;
  align-items: baseline;
  gap: 6px;
}

.msg-author {
  font-size: 13px;
  font-weight: 500;
}

.msg-time {
  font-size: 11px;
  color: #9ca3af;
}

.msg-text {
  font-size: 13px;
}

.chat-input-row {
  margin-top: 8px;
}

.chat-input-wrap {
  display: flex;
  align-items: center;
  background-color: #1e1f22;
  border-radius: 999px;
  padding: 4px 6px 4px 10px;
  border: 1px solid #202225;
}

.chat-input {
  flex: 1;
  min-width: 0;
  background: transparent;
  border: none;
  color: #e5e7eb;
  font-size: 13px;
  outline: none;
}

.chat-input::placeholder {
  color: #9ca3af;
}

.chat-input-actions {
  display: inline-flex;
  align-items: center;
  gap: 4px;
}

.icon-btn-sm {
  width: 28px;
  height: 28px;
  border-radius: 999px;
  border: none;
  background-color: #313338;
  color: #e5e7eb;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  font-size: 14px;
  cursor: pointer;
}

.icon-btn-sm:hover {
  background-color: #3f4248;
}

.send-btn {
  background-color: #5865f2;
  color: #ffffff;
}

.send-btn:hover {
  background-color: #4e5de0;
}

/* VOICE */

.home-voice {
  display: flex;
  flex-direction: column;
  flex: 1;
  min-width: 0;
  min-height: 0;
}

.voice-layout {
  width: 100%;
  display: grid;
  grid-template-columns: minmax(0, 1.2fr) minmax(0, 1fr);
  gap: 12px;
}

.voice-left,
.voice-right {
  min-height: 0;
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.voice-card,
.voice-status-card {
  background-color: #2b2d31;
  border-radius: 12px;
  padding: 10px 12px;
  border: 1px solid #202225;
  min-height: 0;
}

.voice-card-head {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 6px;
}

.voice-card-head h5 {
  font-size: 14px;
  margin: 0;
}

.voice-member-list {
  list-style: none;
  padding: 0;
  margin: 0;
  display: flex;
  flex-direction: column;
  gap: 6px;
  overflow-y: auto;
}

.voice-member-item {
  display: flex;
  align-items: center;
  gap: 8px;
}

.voice-avatar {
  width: 30px;
  height: 30px;
  border-radius: 999px;
  background-color: #1e1f22;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 14px;
  font-weight: 600;
  flex-shrink: 0;
}

.voice-main {
  flex: 1;
  min-width: 0;
}

.voice-name {
  font-size: 13px;
  font-weight: 500;
  display: flex;
  align-items: center;
  gap: 4px;
}

.voice-speaking-dot {
  width: 6px;
  height: 6px;
  border-radius: 999px;
  background-color: #22c55e;
}

.voice-meta {
  font-size: 11px;
  color: #9ca3af;
}

.voice-icons {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  color: #e5e7eb;
}

/* screen share */

.screen-share-card {
  background-color: #2b2d31;
  border-radius: 12px;
  padding: 10px 12px;
  border: 1px solid #202225;
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.screen-share-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.screen-share-title {
  font-size: 14px;
  font-weight: 600;
}

.screen-share-meta {
  font-size: 12px;
  color: #9ca3af;
}

.screen-share-pill {
  font-size: 11px;
  padding: 3px 8px;
  border-radius: 999px;
  background-color: #1e1f22;
  color: #e5e7eb;
}

.screen-share-preview {
  margin-top: 4px;
}

.screen-share-sim {
  width: 100%;
  border-radius: 10px;
  background-color: #111827;
  border: 1px solid #1f2937;
  overflow: hidden;
}

.screen-toolbar {
  height: 26px;
  padding: 0 8px;
  display: flex;
  align-items: center;
  gap: 4px;
  background-color: #020617;
  border-bottom: 1px solid #1f2937;
}

.screen-toolbar .dot {
  width: 7px;
  height: 7px;
  border-radius: 999px;
  display: inline-block;
}

.screen-toolbar .red {
  background-color: #f97373;
}

.screen-toolbar .yellow {
  background-color: #facc15;
}

.screen-toolbar .green {
  background-color: #22c55e;
}

.screen-toolbar-title {
  margin-left: 8px;
  font-size: 11px;
  color: #9ca3af;
}

.screen-content {
  display: grid;
  grid-template-columns: minmax(0, 2fr) minmax(0, 1fr);
  gap: 4px;
  padding: 6px;
  height: 150px;
}

.screen-block {
  border-radius: 6px;
  background-color: #1f2937;
}

.screen-block.main {
  background-image: linear-gradient(135deg, #4f46e5 0%, #22c55e 60%, #111827 100%);
  opacity: 0.35;
}

.screen-block.side {
  background-color: #020617;
  border: 1px dashed #1f2937;
}

.screen-share-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 6px;
  margin-top: 2px;
}

.screen-btn {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  font-size: 12px;
  padding: 4px 8px;
  border-radius: 999px;
  border: 1px solid #3f4248;
  background-color: #1e1f22;
  color: #e5e7eb;
  cursor: pointer;
}

.screen-btn:hover {
  background-color: #3f4248;
}

.screen-btn.danger {
  border-color: #ef4444;
  color: #fecaca;
}

/* voice status */

.voice-status-card {
  display: flex;
  align-items: center;
  gap: 10px;
}

.voice-wave {
  width: 48px;
  height: 48px;
  border-radius: 16px;
  background-color: #1e1f22;
}

.voice-status-text {
  flex: 1;
  min-width: 0;
}

.voice-status-title {
  font-size: 14px;
  font-weight: 600;
}

.voice-status-sub {
  font-size: 12px;
  color: #9ca3af;
}

.screen-request-btn {
  margin-top: 6px;
  display: inline-flex;
  align-items: center;
  gap: 6px;
  font-size: 12px;
  padding: 4px 10px;
  border-radius: 999px;
  border: 1px solid #3f4248;
  background-color: #1e1f22;
  color: #e5e7eb;
  cursor: pointer;
}

.screen-request-btn:hover {
  background-color: #3f4248;
}

/* responsive */

@media (max-width: 992px) {
  .hub-layout,
  .voice-layout {
    grid-template-columns: minmax(0, 1fr);
  }
}

@media (max-width: 768px) {
  .home-header {
    padding-inline: 10px;
  }
  .home-body {
    padding-inline: 8px;
  }
  .hub-block,
  .voice-card,
  .voice-status-card,
  .screen-share-card {
    padding-inline: 10px;
  }
  .screen-content {
    height: 120px;
  }
}
</style>
