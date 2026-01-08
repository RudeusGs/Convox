<template>
  <!-- overlay -->
  <div class="create-modal-backdrop">
    <div class="create-modal">
      <!-- header -->
      <div class="modal-header-custom mb-3">
        <div class="d-flex align-items-center gap-2">
          <div>
            <h5 class="mb-0 fw-semibold">Tạo phòng mới</h5>
            <small class="text-subtle">
              Đặt tên & chọn ảnh đại diện để mọi người dễ nhận ra phòng của bạn.
            </small>
          </div>
        </div>

        <button
          type="button"
          class="btn btn-sm btn-link text-subtle p-0"
          @click="$emit('close')"
        >
          ✕
        </button>
      </div>

      <div class="modal-body-custom">
        <!-- upload + info -->
        <div class="row g-3">
          <!-- Upload avatar -->
          <div class="col-12 col-md-4">
            <div class="section-title small text-subtle mb-2">
              Ảnh phòng
            </div>

            <div
              class="upload-card d-flex flex-column align-items-center justify-content-center"
              @click="triggerFile"
            >
              <div v-if="avatarPreview" class="upload-preview">
                <img :src="avatarPreview" alt="avatar" />
              </div>

              <template v-else>
                <div class="upload-icon mb-2">📁</div>
                <div class="upload-label">Chọn ảnh</div>
                <div class="upload-hint">PNG, JPG, tối đa 5MB</div>
              </template>
            </div>

            <input
              ref="fileInput"
              type="file"
              accept="image/*"
              class="d-none"
              @change="onFileChange"
            />
          </div>

          <!-- Thông tin cơ bản -->
          <div class="col-12 col-md-8">
            <div class="section-title small text-subtle mb-2">
              Thông tin cơ bản
            </div>

            <div class="mb-3">
              <label class="form-label small mb-1 text-subtle fw-semibold">
                Tên phòng <span class="text-danger">*</span>
              </label>
              <input
                v-model="serverName"
                type="text"
                class="form-control create-input"
                placeholder="Ví dụ: Góc Chill, Team Frontend..."
              />
            </div>

            <!-- Bảo mật -->
            <div class="section-title small text-subtle mb-1">
              Bảo mật phòng
            </div>

            <div class="security-card mb-2">
              <div class="d-flex align-items-center justify-content-between mb-2">
                <div>
                  <div class="small fw-semibold">Phòng có mật khẩu</div>
                  <div class="small text-subtle">
                    Chỉ ai có mật khẩu mới vào được.
                  </div>
                </div>

                <div class="form-check form-switch m-0">
                  <input
                    class="form-check-input"
                    type="checkbox"
                    id="hasPasswordSwitch"
                    v-model="hasPassword"
                  />
                </div>
              </div>

              <transition name="fade">
                <div v-if="hasPassword">
                  <label class="form-label small mb-1 text-subtle fw-semibold">
                    Mật khẩu
                  </label>
                  <input
                    v-model="serverPassword"
                    type="password"
                    class="form-control create-input"
                    placeholder="Nhập mật khẩu phòng"
                  />
                </div>
              </transition>
            </div>
          </div>
        </div>

        <div v-if="error" class="text-danger small mt-1 mb-2">
          {{ error }}
        </div>

        <p class="small text-subtle mt-2 mb-0">
          Bằng việc tạo phòng, bạn đồng ý tuân thủ các
          <a href="#" class="link-highlight text-decoration-none">quy tắc cộng đồng</a>
          của hệ thống.
        </p>
      </div>

      <!-- footer -->
      <div class="modal-footer-custom">
        <!-- BÊN TRÁI: join bằng mã -->
        <div class="footer-left d-none d-md-flex align-items-center gap-1">
          <span class="small text-subtle">Muốn tham gia phòng?</span>
          <button
            type="button"
            class="btn-link-code small"
            @click="$emit('open-join-by-code')"
          >
            Nhập mã tham gia
          </button>
        </div>

        <!-- mobile: chỉ hiện link nhỏ -->
        <div class="footer-left-mobile d-flex d-md-none mb-2 w-100">
          <button
            type="button"
            class="btn-link-code small ms-auto"
            @click="$emit('open-join-by-code')"
          >
            Nhập mã tham gia
          </button>
        </div>

        <!-- BÊN PHẢI: nút hành động -->
        <div class="footer-actions ms-auto">
          <button
            type="button"
            class="btn btn-outline-light btn-ghost"
            @click="$emit('close')"
          >
            Huỷ
          </button>

          <button
            type="button"
            class="btn btn-primary btn-create"
            @click="handleCreate"
          >
            Tạo phòng
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'CreateServerModal',
  emits: ['close', 'create', 'open-join-by-code'],
  data() {
    return {
      serverName: '',
      avatarFile: null,
      avatarPreview: null,
      hasPassword: false,
      serverPassword: '',
      error: '',
    }
  },
  methods: {
    triggerFile() {
      this.$refs.fileInput.click()
    },
    onFileChange(e) {
      const file = e.target.files[0]
      if (!file) return
      this.avatarFile = file
      this.avatarPreview = URL.createObjectURL(file)
    },
    handleCreate() {
      if (!this.serverName.trim()) {
        this.error = 'Vui lòng nhập tên phòng.'
        return
      }
      if (this.hasPassword && !this.serverPassword.trim()) {
        this.error = 'Bạn đã bật mật khẩu, hãy nhập mật khẩu cho phòng.'
        return
      }

      this.error = ''

      this.$emit('create', {
        name: this.serverName.trim(),
        avatarFile: this.avatarFile,
        avatarPreview: this.avatarPreview,
        hasPassword: this.hasPassword,
        password: this.hasPassword ? this.serverPassword.trim() : null,
      })
    },
  },
}
</script>

<style scoped>
.create-modal-backdrop {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.8);
  z-index: 1050;
  display: flex;
  align-items: center;
  justify-content: center;
}

.create-modal {
  width: 100%;
  max-width: 540px;
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

.section-title {
  text-transform: uppercase;
  letter-spacing: 0.08em;
}

.text-subtle {
  color: #9ca3af !important;
}

.upload-card {
  width: 100%;
  aspect-ratio: 1 / 1;
  border-radius: 18px;
  background: #111827;
  border: 1px dashed rgba(156, 163, 175, 0.6);
  cursor: pointer;
  transition: all 0.15s ease;
  text-align: center;
  padding: 10px;
}

.upload-card:hover {
  border-style: solid;
  border-color: #6366f1;
  background: radial-gradient(circle at top left, rgba(99, 102, 241, 0.2), #111827);
}

.upload-icon {
  font-size: 26px;
}

.upload-label {
  font-size: 13px;
  font-weight: 600;
  margin-bottom: 2px;
}

.upload-hint {
  font-size: 11px;
  color: #9ca3af;
}

.upload-preview {
  width: 100%;
  height: 100%;
  border-radius: 14px;
  overflow: hidden;
}

.upload-preview img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.create-input {
  background-color: #111827;
  border: 1px solid #111827;
  color: #ffffff;
  font-size: 14px;
}

.create-input::placeholder {
  color: #ffffff;
}

.create-input:focus {
  background-color: #020617;
  border-color: #6366f1;
  box-shadow: 0 0 0 1px #6366f1;
  color: #ffffff;
}

.security-card {
  background: #111827;
  border-radius: 14px;
  padding: 10px 12px;
  border: 1px solid rgba(148, 163, 184, 0.25);
}

/* footer */
.modal-footer-custom {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 8px;
  margin-top: 12px;
  border-top: 1px solid rgba(148, 163, 184, 0.25);
  padding-top: 10px;
}

.footer-left {
  flex: 1;
}

.footer-left-mobile {
  order: -1;
}

.footer-actions {
  display: flex;
  gap: 10px;
}

/* nút link "Nhập mã tham gia" */
.btn-link-code {
  background: none;
  border: none;
  color: #6366f1;
  padding: 0;
  cursor: pointer;
}

.btn-link-code:hover {
  text-decoration: underline;
}

/* nút footer mặc định */
.btn-ghost {
  border-radius: 999px;
  border-color: rgba(148, 163, 184, 0.5);
  color: #e5e7eb;
  background: transparent;
  padding-inline: 16px;
}

.btn-ghost:hover {
  background: rgb(218, 47, 17);
  border-color: #020617;
  color: #ffffff;
}

.btn-create {
  border-radius: 999px;
  background: #5865f2;
  border: none;
  padding-inline: 22px;
  font-weight: 600;
}

.btn-create:hover {
  background: #4654ef;
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.15s ease;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
