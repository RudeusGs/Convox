<template>
  <!-- overlay -->
  <div class="join-modal-backdrop">
    <div class="join-modal">
      <!-- header -->
      <div class="modal-header-custom mb-3">
        <div>
          <h5 class="mb-0 fw-semibold">Tham gia phòng bằng mã</h5>
          <small class="text-subtle">
            Dán mã phòng mà admin / bạn bè gửi cho bạn để vào chung.
          </small>
        </div>

        <button
          type="button"
          class="btn btn-sm btn-link text-subtle p-0"
          @click="$emit('close')"
        >
          ✕
        </button>
      </div>

      <!-- body -->
      <div class="modal-body-custom">
        <div class="mb-3">
          <label class="form-label small mb-1 text-subtle fw-semibold">
            Mã phòng <span class="text-danger">*</span>
          </label>
          <input
            v-model="joinCode"
            type="text"
            class="form-control join-input"
            placeholder="Ví dụ: AB12CD"
            @keyup.enter="handleJoin"
          />
          <small class="text-subtle d-block mt-1">
            Mã thường gồm 5–8 ký tự, phân biệt chữ hoa / thường tuỳ hệ thống của bạn.
          </small>
        </div>

        <div class="mb-2">
          <label class="form-label small mb-1 text-subtle fw-semibold">
            Biệt danh trong phòng (tuỳ chọn)
          </label>
          <input
            v-model="nickname"
            type="text"
            class="form-control join-input"
            placeholder="Tên hiển thị trong phòng (nếu khác với tài khoản)"
            @keyup.enter="handleJoin"
          />
        </div>

        <div v-if="error" class="text-danger small mt-1 mb-1">
          {{ error }}
        </div>

        <p class="small text-subtle mt-2 mb-0">
          Không có mã? Hỏi admin phòng hoặc chủ server gửi cho bạn.
        </p>
      </div>

      <!-- footer -->
      <div class="modal-footer-custom">
        <button
          type="button"
          class="btn btn-outline-light btn-ghost"
          @click="$emit('close')"
        >
          Huỷ
        </button>

        <button
          type="button"
          class="btn btn-primary btn-join"
          @click="handleJoin"
        >
          Tham gia
        </button>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'ModalJoinRoom',
  emits: ['close', 'join'],
  data() {
    return {
      joinCode: '',
      nickname: '',
      error: '',
    }
  },
  methods: {
    handleJoin() {
      this.error = ''

      if (!this.joinCode.trim()) {
        this.error = 'Vui lòng nhập mã phòng để tham gia.'
        return
      }

      this.$emit('join', {
        code: this.joinCode.trim(),
        nickname: this.nickname.trim() || null,
      })
    },
  },
}
</script>

<style scoped>
.join-modal-backdrop {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.8);
  z-index: 1060;
  display: flex;
  align-items: center;
  justify-content: center;
}

.join-modal {
  width: 100%;
  max-width: 460px;
  background: #20232b;
  border-radius: 18px;
  padding: 18px 20px 16px;
  color: #f9fafb;
  box-shadow: 0 18px 50px rgba(0, 0, 0, 0.8);
  border: 1px solid rgba(148, 163, 184, 0.25);
}

.modal-header-custom {
  display: flex;
  align-items: center;
  justify-content: space-between;
  border-bottom: 1px solid rgba(148, 163, 184, 0.25);
  padding-bottom: 10px;
  margin-bottom: 12px;
}

.modal-body-custom {
  font-size: 14px;
}

.text-subtle {
  color: #9ca3af !important;
}

/* input */
.join-input {
  background-color: #111827;
  border-radius: 8px;
  border: 1px solid #111827;
  color: #ffffff;
  font-size: 14px;
  padding: 8px 10px;
}

.join-input::placeholder {
  color: #6b7280;
}

.join-input:focus {
  background-color: #020617;
  border-color: #6366f1;
  color: #ffffff;
  box-shadow: 0 0 0 1px #6366f1;
  outline: none;
}

/* footer */
.modal-footer-custom {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
  margin-top: 12px;
  border-top: 1px solid rgba(148, 163, 184, 0.25);
  padding-top: 10px;
}

.btn-ghost {
  border-radius: 999px;
  border-color: rgba(148, 163, 184, 0.5);
  color: #e5e7eb;
  background: transparent;
  padding-inline: 16px;
}

.btn-ghost:hover {
  background: #111827;
  border-color: #020617;
  color: #ffffff;
}

.btn-join {
  border-radius: 999px;
  background: #6366f1;
  border: none;
  padding-inline: 22px;
  font-weight: 600;
}

.btn-join:hover {
  background: #4f46e5;
}

/* error text */
.text-danger {
  color: #f87171 !important;
}

/* responsive */
@media (max-width: 576px) {
  .join-modal {
    max-width: 94%;
    padding-inline: 16px;
  }
}
</style>
