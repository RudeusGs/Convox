<template>
  <div class="nav-tabs">
    <button 
      v-for="tab in tabs" 
      :key="tab.id"
      class="nav-tab"
      :class="{ active: activeTab === tab.id }"
      @click="$emit('setActiveTab', tab.id)"
      :title="tab.label"
    >
      <i class="bi" :class="tab.icon"></i>
      <div v-if="tab.count" class="tab-badge">{{ tab.count }}</div>
    </button>
  </div>
</template>

<script setup>
defineProps({
  tabs: {
    type: Array,
    required: true
  },
  activeTab: {
    type: String,
    required: true
  }
})

defineEmits(['setActiveTab'])
</script>

<style scoped>
.nav-tabs {
  padding: 12px 16px;
  display: flex;
  gap: 4px;
  border-bottom: 1px solid #2b2d31;
  flex-shrink: 0;
}

.nav-tab {
  flex: 1;
  padding: 8px;
  background: none;
  border: none;
  border-radius: 6px;
  color: #b5bac1;
  cursor: pointer;
  transition: all 0.15s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
  font-size: 16px;
  min-height: 36px;
}

.nav-tab:hover {
  background: #35373c;
  color: #f2f3f5;
}

.nav-tab.active {
  background: #5865f2;
  color: white;
}

.tab-badge {
  position: absolute;
  top: 2px;
  right: 2px;
  background: #ed4245;
  color: white;
  font-size: 10px;
  padding: 2px 5px;
  border-radius: 8px;
  min-width: 16px;
  text-align: center;
  line-height: 1;
}
</style>
