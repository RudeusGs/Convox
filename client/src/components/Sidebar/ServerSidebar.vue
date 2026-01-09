<template>
  <aside class="server-sidebar d-flex flex-column">
    <div class="sidebar-header">
      <div class="server-title-wrap">
        <div class="server-title">
          {{ server ? server.name : 'Chưa chọn không gian học' }}
        </div>
        <div class="server-subtitle">
          {{
            server
              ? server.tagline || 'Chọn phòng chung hoặc phòng nhóm để bắt đầu'
              : 'Chọn một máy chủ ở cột bên trái'
          }}
        </div>
      </div>

      <div class="header-actions" v-if="server">
        <button type="button" class="header-icon-btn">
          <i class="bi bi-three-dots"></i>
        </button>
      </div>
    </div>

    <button
      v-if="server"
      type="button"
      class="collapse-fab"
      @click="emitClose"
    >
      <i class="bi bi-chevron-left"></i>
    </button>

    <div v-if="server" class="sidebar-scroll">
      <div v-if="mainRoom" class="class-hub">
        <div class="class-hub-title">Phòng chung của lớp</div>
        <button
          type="button"
          class="class-hub-card"
          :class="{ 'class-hub-card-active': activeChannelId === mainRoom.id }"
          @click="handleSelectChannel(mainRoom)"
        >
          <div class="class-hub-icon">
            <i class="bi bi-people-fill"></i>
          </div>
          <div class="class-hub-main">
            <div class="class-hub-name">
              {{ mainRoom.name }}
            </div>
            <div class="class-hub-desc">
              {{ mainRoom.topic || 'Giáo viên và toàn bộ học sinh đều có mặt ở đây.' }}
            </div>
          </div>
          <div class="class-hub-badge">
            Lớp chính
          </div>
        </button>
      </div>

      <div
        v-for="section in groupSections"
        :key="section.id"
        class="sidebar-section"
      >
        <div class="section-label">
          <span>{{ section.title }}</span>
          <div class="section-right">
            <span
              v-if="section.type === 'group-text'"
              class="section-pill"
            >
              nhóm chat
            </span>
            <span
              v-else-if="section.type === 'group-voice'"
              class="section-pill section-pill-voice"
            >
              nhóm voice
            </span>

            <button
              v-if="section.canAdd"
              type="button"
              class="section-add"
            >
              +
            </button>
          </div>
        </div>

        <button
          v-for="ch in section.channels"
          :key="ch.id"
          type="button"
          class="channel-item"
          :class="[
            { 'channel-item-active': activeChannelId === ch.id },
            ch.type === 'voice' ? 'channel-voice' : 'channel-text'
          ]"
          @click="handleSelectChannel(ch)"
        >
          <span class="channel-icon">
            <i
              v-if="ch.type === 'voice'"
              class="bi bi-headset"
            ></i>
            <i
              v-else
              class="bi bi-chat-dots"
            ></i>
          </span>

          <div class="channel-main">
            <span class="channel-name">
              {{ ch.name }}
            </span>
            <span
              v-if="ch.topic"
              class="channel-topic"
            >
              {{ ch.topic }}
            </span>
          </div>

          <button
            v-if="ch.type === 'voice'"
            type="button"
            class="channel-cta"
            @click.stop="$emit('join-voice', ch)"
          >
            <i class="bi bi-soundwave"></i>
          </button>

          <i
            v-else-if="ch.canConfig"
            class="bi bi-sliders channel-config"
          ></i>
        </button>
      </div>
    </div>

    <div v-else class="sidebar-empty">
      <p>Chọn một không gian học ở cột bên trái để xem phòng chung và các phòng nhóm.</p>
    </div>
  </aside>
</template>

<script>
export default {
  name: 'ServerSidebar',
  props: {
    server: {
      type: Object,
      default: null,
    },
    activeChannelId: {
      type: String,
      default: null,
    },
  },
  emits: ['select-channel', 'join-voice', 'close-sidebar'],
  computed: {
    mainRoom() {
      if (this.server) {
        if (this.server.mainRoom) {
          return this.server.mainRoom
        }

        if (Array.isArray(this.server.sections)) {
          const allChannels = this.server.sections.flatMap(s => s.channels || [])
          if (allChannels.length) {
            return allChannels[0]
          }
        }
      }

      return {
        id: 'class-main',
        name: 'Lớp học chung',
        type: 'text',
        topic: 'Kênh thông báo & trao đổi chung cho cả lớp.',
      }
    },
    groupSections() {
      if (
        this.server &&
        Array.isArray(this.server.sections) &&
        this.server.sections.length
      ) {
        const mainId = this.mainRoom && this.mainRoom.id
        return this.server.sections
          .map((section, idx) => {
            const mappedType =
              section.type === 'voice'
                ? 'group-voice'
                : 'group-text'

            return {
              id: section.id || `section-${idx}`,
              title: section.title || 'Nhóm học',
              type: mappedType,
              canAdd: !!section.canAdd,
              channels: (section.channels || []).filter(ch => ch.id !== mainId),
            }
          })
          .filter(sec => sec.channels.length)
      }

      return [
        {
          id: 'group-chat',
          title: 'Nhóm học bài',
          type: 'group-text',
          canAdd: true,
          channels: [
            {
              id: 'group-a',
              name: 'Nhóm A – Toán',
              type: 'text',
              topic: 'Làm bài tập và ôn luyện Toán.',
            },
            {
              id: 'group-b',
              name: 'Nhóm B – Văn',
              type: 'text',
              topic: 'Thảo luận dàn ý, bài luận.',
            },
          ],
        },
        {
          id: 'group-voice',
          title: 'Phòng học voice',
          type: 'group-voice',
          canAdd: true,
          channels: [
            {
              id: 'voice-focus',
              name: 'Focus cùng nhau',
              type: 'voice',
              topic: 'Bật mic khi cần, còn lại tập trung làm bài.',
            },
            {
              id: 'voice-review',
              name: 'Ôn kiểm tra',
              type: 'voice',
              topic: 'Hỏi đáp nhanh trước giờ kiểm tra.',
            },
          ],
        },
      ]
    },
  },
  methods: {
    handleSelectChannel(ch) {
      this.$emit('select-channel', ch)
    },
    emitClose() {
      this.$emit('close-sidebar')
    },
  },
}
</script>

<style scoped>
.server-sidebar {
  width: 260px;
  min-height: 100vh;
  background-color: #2b2d31;
  color: #e5e7eb;
  border-right: 1px solid #1f2933;
  display: flex;
  flex-direction: column;
  position: relative;
}

.sidebar-header {
  height: 60px;
  padding: 8px 12px 8px 14px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  border-bottom: 1px solid #1f2933;
}

.server-title-wrap {
  min-width: 0;
}

.server-title {
  font-size: 14px;
  font-weight: 600;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.server-subtitle {
  font-size: 11px;
  color: #9ca3af;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.header-actions {
  display: inline-flex;
  align-items: center;
  gap: 4px;
}

.header-icon-btn {
  border: none;
  background: transparent;
  color: #9ca3af;
  padding: 4px;
  border-radius: 999px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  font-size: 14px;
  margin-right: 4px;
}

.header-icon-btn:hover {
  color: #ffffff;
  background-color: #383a40;
}

.collapse-fab {
  position: absolute;
  top: 14px;
  right: -12px;
  width: 26px;
  height: 26px;
  border-radius: 999px;
  border: 1px solid #1f2933;
  background-color: #1e1f22;
  color: #e5e7eb;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  font-size: 14px;
  cursor: pointer;
  box-shadow: 0 8px 18px rgba(0, 0, 0, 0.8);
}

.collapse-fab:hover {
  background-color: #2b2d31;
  color: #ffffff;
}

.sidebar-scroll {
  flex: 1;
  padding: 10px 10px 14px;
  overflow-y: auto;
  font-size: 13px;
}

.class-hub {
  margin-bottom: 10px;
}

.class-hub-title {
  font-size: 11px;
  text-transform: uppercase;
  letter-spacing: 0.08em;
  color: #9ca3af;
  margin: 0 2px 4px;
}

.class-hub-card {
  width: 100%;
  border: 1px solid #1f2937;
  border-radius: 10px;
  padding: 8px 10px;
  background-color: #020617;     
  color: #e5e7eb;                
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
  box-shadow: 0 6px 16px rgba(0, 0, 0, 0.5);
  transition: transform 0.08s ease, box-shadow 0.08s ease, background-color 0.1s ease;
}

.class-hub-card:hover {
  transform: translateY(-1px);
  background-color: #111827;
  box-shadow: 0 10px 24px rgba(0, 0, 0, 0.7);
}

.class-hub-card-active {
  background-color: #5865f2;    
  border-color: #5865f2;
  box-shadow: 0 12px 30px rgba(0, 0, 0, 0.9);
}

.class-hub-card-active .class-hub-name {
  color: #ffffff;
}

.class-hub-card-active .class-hub-desc {
  color: #e5e7eb;
}

.class-hub-card-active .class-hub-icon {
  background-color: rgba(15, 23, 42, 0.3);
  color: #e0e7ff;
}

.class-hub-icon {
  width: 30px;
  height: 30px;
  border-radius: 999px;
  background-color: #111827;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 16px;
  color: #a5b4fc;
  flex-shrink: 0;
}

.class-hub-main {
  flex: 1;
  min-width: 0;
}

.class-hub-name {
  font-size: 13px;
  font-weight: 600;
  color: #f9fafb;              
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.class-hub-desc {
  font-size: 11px;
  color: #d1d5db;               
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.class-hub-badge {
  font-size: 10px;
  padding: 2px 8px;
  border-radius: 999px;
  background-color: #111827;
  color: #c4f1ff;
  border: 1px solid #374151;
}

.sidebar-section + .sidebar-section {
  margin-top: 12px;
}

.section-label {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 4px;
  margin-bottom: 2px;
  text-transform: uppercase;
  font-size: 11px;
  letter-spacing: 0.08em;
  color: #9ca3af;
}

.section-right {
  display: inline-flex;
  align-items: center;
  gap: 4px;
}

.section-pill {
  font-size: 10px;
  padding: 1px 6px;
  border-radius: 999px;
  background-color: #1e1f22;
  color: #9ca3af;
}

.section-pill-voice {
  background-color: #023430;
  color: #bbf7d0;
}

.section-add {
  border: none;
  background: transparent;
  color: #9ca3af;
  font-size: 16px;
  padding: 0 4px;
  border-radius: 999px;
}

.section-add:hover {
  color: #ffffff;
  background-color: #383a40;
}

.channel-item {
  width: 100%;
  border: none;
  background: transparent;
  display: flex;
  align-items: center;
  gap: 6px;
  border-radius: 8px;
  padding: 5px 7px;
  color: #d1d5db;
  cursor: pointer;
  transition: background-color 0.08s ease, transform 0.06s ease;
}

.channel-item:hover {
  background-color: #383a40;
  color: #ffffff;
}

.channel-item-active {
  background-color: #404249;
  color: #ffffff;
  transform: translateX(1px);
}

.channel-text .channel-icon {
  color: #b0b3b8;
}

.channel-voice .channel-icon {
  color: #38bdf8;
}

.channel-icon {
  width: 18px;
  text-align: center;
  font-size: 14px;
}

.channel-main {
  flex: 1;
  min-width: 0;
  display: flex;
  flex-direction: column;
}

.channel-name {
  text-align: left;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.channel-topic {
  font-size: 11px;
  color: #9ca3af;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.channel-cta {
  border: none;
  background-color: #023430;
  color: #6ee7b7;
  border-radius: 999px;
  width: 24px;
  height: 24px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  font-size: 13px;
  padding: 0;
}

.channel-cta:hover {
  background-color: #065f46;
  color: #a7f3d0;
}

.channel-config {
  font-size: 14px;
  color: #9ca3af;
}

.channel-item:hover .channel-config {
  color: #ffffff;
}

.sidebar-empty {
  flex: 1;
  padding: 16px 12px;
  font-size: 13px;
  color: #9ca3af;
}

@media (max-width: 768px) {
  .server-sidebar {
    width: 230px;
  }

  .collapse-fab {
    right: -10px;
    top: 12px;
    width: 24px;
    height: 24px;
  }
}
</style>
