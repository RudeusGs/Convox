<template>
  <button
    class="server-btn d-flex align-items-center justify-content-center mb-2 border-0"
    :class="{ 'server-btn-active': isActive }"
    type="button"
    @click="$emit('select')"
  >
    <img
      v-if="avatar"
      :src="avatar"
      alt=""
      class="w-100 h-100 server-img"
    />
    <span v-else class="fw-semibold text-light">
      {{ label?.charAt(0)?.toUpperCase() }}
    </span>
  </button>
</template>

<script>
export default {
  name: 'ServerBarItem',
  props: {
    label: {
      type: String,
      default: '',
    },
    avatar: {
      type: String,
      default: null,
    },
    isActive: {
      type: Boolean,
      default: false,
    },
  },
}
</script>

<style scoped>
.server-btn {
  width: 48px;
  height: 48px;
  border-radius: 16px;
  background-color: #313338;
  padding: 0;
  transition: all 0.15s ease;
}

.server-btn:hover {
  border-radius: 18px;
  background-color: #5865f2;
}

.server-btn-active {
  background-color: #5865f2;
  border-radius: 18px;
}

.server-img {
  object-fit: cover;
  border-radius: 16px;
}
</style>
