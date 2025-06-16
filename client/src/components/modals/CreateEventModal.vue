<template>
  <div v-if="isOpen" class="modal-overlay" @click="closeModal">
    <div class="modal-container" @click.stop>
      <div class="modal-header">
        <h2>Tạo sự kiện</h2>
        <button class="close-btn" @click="closeModal">
          <i class="bi bi-x"></i>
        </button>
      </div>
      
      <div class="modal-content">
        <form @submit.prevent="createEvent">
          <div class="form-group">
            <label for="event-name">Tên sự kiện *</label>
            <input 
              id="event-name"
              v-model="eventData.name" 
              type="text" 
              class="form-input" 
              placeholder="Nhập tên sự kiện..."
              required
            />
          </div>
          
          <div class="form-group">
            <label for="event-description">Mô tả</label>
            <textarea 
              id="event-description"
              v-model="eventData.description" 
              class="form-textarea" 
              placeholder="Mô tả về sự kiện..."
              rows="3"
            ></textarea>
          </div>
          
          <div class="form-row">
            <div class="form-group">
              <label for="event-date">Ngày *</label>
              <input 
                id="event-date"
                v-model="eventData.date" 
                type="date" 
                class="form-input"
                required
              />
            </div>
            
            <div class="form-group">
              <label for="event-time">Thời gian *</label>
              <input 
                id="event-time"
                v-model="eventData.time" 
                type="time" 
                class="form-input"
                required
              />
            </div>
          </div>
          
          <div class="form-group">
            <label for="event-location">Địa điểm</label>
            <select id="event-location" v-model="eventData.location" class="form-select">
              <option value="">Chọn địa điểm...</option>
              <option value="voice">Kênh thoại</option>
              <option value="stage">Stage Channel</option>
              <option value="external">Bên ngoài Discord</option>
            </select>
          </div>
          
          <div v-if="eventData.location === 'voice'" class="form-group">
            <label for="voice-channel">Kênh thoại</label>
            <select id="voice-channel" v-model="eventData.voiceChannel" class="form-select">
              <option value="">Chọn kênh thoại...</option>
              <option v-for="channel in voiceChannels" :key="channel.id" :value="channel.id">
                {{ channel.name }}
              </option>
            </select>
          </div>
          
          <div v-if="eventData.location === 'external'" class="form-group">
            <label for="external-location">Địa điểm bên ngoài</label>
            <input 
              id="external-location"
              v-model="eventData.externalLocation" 
              type="text" 
              class="form-input" 
              placeholder="Nhập địa điểm..."
            />
          </div>
          
          <div class="form-group">
            <label>Tùy chọn</label>
            <div class="checkbox-group">
              <label class="checkbox-item">
                <input type="checkbox" v-model="eventData.notifyMembers" />
                <span class="checkmark"></span>
                Thông báo cho tất cả thành viên
              </label>
              
              <label class="checkbox-item">
                <input type="checkbox" v-model="eventData.allowRSVP" />
                <span class="checkmark"></span>
                Cho phép thành viên xác nhận tham gia
              </label>
              
              <label class="checkbox-item">
                <input type="checkbox" v-model="eventData.recurring" />
                <span class="checkmark"></span>
                Sự kiện lặp lại
              </label>
            </div>
          </div>
          
          <div v-if="eventData.recurring" class="form-group">
            <label for="recurrence">Lặp lại</label>
            <select id="recurrence" v-model="eventData.recurrence" class="form-select">
              <option value="daily">Hàng ngày</option>
              <option value="weekly">Hàng tuần</option>
              <option value="monthly">Hàng tháng</option>
            </select>
          </div>
          
          <div class="form-actions">
            <button type="button" class="btn-secondary" @click="closeModal">
              Hủy
            </button>
            <button type="submit" class="btn-primary" :disabled="!isFormValid">
              Tạo sự kiện
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

const props = defineProps({
  isOpen: Boolean,
  serverName: String
})

const emit = defineEmits(['close', 'eventCreated'])

const eventData = ref({
  name: '',
  description: '',
  date: '',
  time: '',
  location: '',
  voiceChannel: '',
  externalLocation: '',
  notifyMembers: true,
  allowRSVP: true,
  recurring: false,
  recurrence: 'weekly'
})

const voiceChannels = ref([
  { id: 'lobby', name: 'Sảnh chờ' },
  { id: 'meeting', name: 'Họp nhóm' },
  { id: 'casual', name: 'Trò chuyện' },
  { id: 'gaming', name: 'Gaming' }
])

const isFormValid = computed(() => {
  return eventData.value.name.trim() && 
         eventData.value.date && 
         eventData.value.time
})

const createEvent = () => {
  if (!isFormValid.value) return
  
  const newEvent = {
    id: Date.now(),
    ...eventData.value,
    createdAt: new Date(),
    attendees: [],
    status: 'scheduled'
  }
  
  console.log('Creating event:', newEvent)
  emit('eventCreated', newEvent)
  
  // Reset form
  eventData.value = {
    name: '',
    description: '',
    date: '',
    time: '',
    location: '',
    voiceChannel: '',
    externalLocation: '',
    notifyMembers: true,
    allowRSVP: true,
    recurring: false,
    recurrence: 'weekly'
  }
  
  closeModal()
}

const closeModal = () => {
  emit('close')
}
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.85);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 10000;
}

.modal-container {
  background: #36393f;
  border-radius: 8px;
  width: 90vw;
  max-width: 500px;
  max-height: 80vh;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.modal-header {
  padding: 20px;
  border-bottom: 1px solid #202225;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.modal-header h2 {
  color: #ffffff;
  font-size: 20px;
  font-weight: 600;
  margin: 0;
}

.close-btn {
  background: none;
  border: none;
  color: #b9bbbe;
  cursor: pointer;
  padding: 8px;
  border-radius: 4px;
  transition: all 0.15s ease;
  font-size: 20px;
}

.close-btn:hover {
  background: #40444b;
  color: #dcddde;
}

.modal-content {
  flex: 1;
  padding: 20px;
  overflow-y: auto;
}

.form-group {
  margin-bottom: 20px;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
}

.form-group label {
  display: block;
  color: #b9bbbe;
  font-size: 12px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  margin-bottom: 8px;
}

.form-input, .form-textarea, .form-select {
  width: 100%;
  background: #40444b;
  border: 1px solid #202225;
  border-radius: 4px;
  padding: 10px;
  color: #dcddde;
  font-size: 16px;
  outline: none;
  font-family: inherit;
}

.form-input:focus, .form-textarea:focus, .form-select:focus {
  border-color: #5865f2;
}

.form-textarea {
  resize: vertical;
  min-height: 80px;
}

.form-select {
  cursor: pointer;
}

.checkbox-group {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.checkbox-item {
  display: flex;
  align-items: center;
  gap: 12px;
  cursor: pointer;
  color: #dcddde;
  font-size: 14px;
  font-weight: normal;
  text-transform: none;
  letter-spacing: normal;
  margin-bottom: 0;
}

.checkbox-item input[type="checkbox"] {
  display: none;
}

.checkmark {
  width: 20px;
  height: 20px;
  background: #40444b;
  border: 2px solid #72767d;
  border-radius: 4px;
  position: relative;
  transition: all 0.15s ease;
  flex-shrink: 0;
}

.checkbox-item input[type="checkbox"]:checked + .checkmark {
  background: #5865f2;
  border-color: #5865f2;
}

.checkbox-item input[type="checkbox"]:checked + .checkmark::after {
  content: '';
  position: absolute;
  left: 6px;
  top: 2px;
  width: 4px;
  height: 8px;
  border: solid white;
  border-width: 0 2px 2px 0;
  transform: rotate(45deg);
}

.form-actions {
  display: flex;
  gap: 12px;
  justify-content: flex-end;
  margin-top: 24px;
  padding-top: 20px;
  border-top: 1px solid #202225;
}

.btn-secondary, .btn-primary {
  padding: 10px 20px;
  border-radius: 4px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.15s ease;
  border: none;
}

.btn-secondary {
  background: none;
  color: #dcddde;
  border: 1px solid #4f545c;
}

.btn-secondary:hover {
  border-color: #dcddde;
}

.btn-primary {
  background: #5865f2;
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background: #4752c4;
}

.btn-primary:disabled {
  background: #4f545c;
  cursor: not-allowed;
  opacity: 0.6;
}
</style>
