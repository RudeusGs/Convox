<template>
  <div class="voice-user-item">
    <div class="voice-user-avatar">
      <img :src="user.avatar" :alt="user.name" />
      <div class="voice-status">
        <i v-if="user.muted" class="bi bi-mic-mute"></i>
        <i v-if="user.deafened" class="bi bi-volume-mute"></i>
      </div>
    </div>
    <span class="voice-user-name">{{ user.name }}</span>
  </div>
</template>

<script setup>
defineProps({
  user: {
    type: Object,
    required: true
  }
})
</script>

<style scoped>
.voice-user-item {
  display: flex;
  align-items: center;
  padding: 4px 8px;
  margin: 1px 0;
  border-radius: 4px;
  color: #b5bac1;
  font-size: 12px;
  min-height: 28px;
}

.voice-user-item:hover {
  background: #35373c;
  color: #f2f3f5;
}

.voice-user-avatar {
  position: relative;
  margin-right: 6px;
  flex-shrink: 0;
}

.voice-user-avatar img {
  width: 20px;
  height: 20px;
  border-radius: 50%;
}

.voice-status {
  position: absolute;
  bottom: -2px;
  right: -2px;
  display: flex;
  gap: 1px;
}

.voice-status i {
  font-size: 8px;
  color: #ed4245;
  background: #1e1f22;
  border-radius: 2px;
  padding: 1px;
}

.voice-user-name {
  flex: 1;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}
</style>
