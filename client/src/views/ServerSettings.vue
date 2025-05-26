<template>
  <div class="server-settings">
    <div class="settings-header">
      <h2>Server Settings</h2>
    </div>
    <div class="settings-content">
      <p>This is where server settings would go.</p>
      <p>Server ID: {{ serverId }}</p>
    </div>
  </div>
</template>

<script setup>
defineProps({
  serverId: {
    type: String,
    required: true
  }
})
</script>

<style scoped>
.server-settings {
  flex: 1;
  display: flex;
  flex-direction: column;
  background: #36393f;
  color: #dcddde;
  padding: 20px;
}

.settings-header {
  margin-bottom: 20px;
  border-bottom: 1px solid #202225;
  padding-bottom: 10px;
}

.settings-header h2 {
  font-size: 20px;
  font-weight: 600;
  margin: 0;
}

.settings-content {
  font-size: 16px;
  line-height: 1.5;
}
</style>
