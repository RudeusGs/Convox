<template>
  <div class="register-page">
    <div class="register-shell">
      <div class="register-side d-none d-md-flex flex-column justify-content-between">
        <div>
          <h1 class="brand-title">Convox</h1>
          <p class="brand-subtitle">
            Tạo tài khoản, vào room, nói chuyện với team như ở chung một phòng.
          </p>

          <div class="fun-tag">
            <span class="dot-online"></span>
            <span>user mới vừa join server #welcome</span>
          </div>

          <div class="fun-chat-card">
            <div class="fun-chat-header">
              <span class="room-hash">#</span>
              <span class="room-name">onboarding</span>
              <span class="room-pill">auto welcome</span>
            </div>

            <div class="fun-chat-body">
              <div class="fun-msg">
                <div class="fun-msg-name">
                  <span class="name">Bot</span>
                  <span class="time">10:02</span>
                </div>
                <div class="fun-msg-text">
                  @bạn vừa tạo tài khoản. Vô room giới thiệu cái coi 👋
                </div>
              </div>

              <div class="fun-msg">
                <div class="fun-msg-name">
                  <span class="name">Bạn</span>
                  <span class="time">10:03</span>
                </div>
                <div class="fun-msg-text">
                  Xin chào, em hứa sẽ không spam meme trong kênh serious ạ.
                </div>
              </div>

              <div class="fun-msg typing">
                <div class="fun-msg-name">
                  <span class="name">Cả server</span>
                  <span class="time">đang gõ...</span>
                </div>
                <div class="typing-dots">
                  <span></span><span></span><span></span>
                </div>
              </div>
            </div>
          </div>

          <ul class="fun-list">
            <li>🪪 Một tài khoản cho mọi server Convox.</li>
            <li>🧩 Tách biệt user và room, dễ quản lý team.</li>
            <li>🕶️ Vào room nào, vibe room đó, không lẫn lộn.</li>
          </ul>
        </div>

        <div class="brand-note">
          <p>Đặt tên cho tử tế, sau này xóa lịch sử chat vẫn còn nick.</p>
        </div>
      </div>

      <div class="register-main">
        <button type="button" class="back-button" @click="goBack">
          <span class="back-icon">←</span>
          <span>Quay về</span>
        </button>

        <div class="register-card">
          <div class="register-header mb-3">
            <h3 class="mb-1">Tạo tài khoản</h3>
            <p class="mb-0 small">
              Điền thông tin bên dưới để tham gia các phòng chat trên Convox.
            </p>
          </div>

          <form @submit.prevent="submitRegister">
            <div class="row-2col">
              <div class="field-group">
                <label class="form-label small">Họ tên</label>
                <input
                  v-model="form.fullName"
                  type="text"
                  class="form-control register-input"
                  placeholder="Nguyễn Văn A"
                />
              </div>
              <div class="field-group">
                <label class="form-label small">Email</label>
                <input
                  v-model="form.email"
                  type="email"
                  class="form-control register-input"
                  placeholder="ban@example.com"
                />
              </div>
            </div>

            <div class="row-2col">
              <div class="field-group">
                <label class="form-label small">Tên đăng nhập</label>
                <input
                  v-model="form.userName"
                  type="text"
                  class="form-control register-input"
                  placeholder="username"
                />
              </div>
              <div class="field-group">
                <label class="form-label small">Mật khẩu</label>
                <input
                  v-model="form.password"
                  type="password"
                  class="form-control register-input"
                  placeholder="Mật khẩu"
                />
              </div>
            </div>

            <div class="mb-2">
              <label class="form-label small">Nhập lại mật khẩu</label>
              <input
                v-model="form.confirmPassword"
                type="password"
                class="form-control register-input"
                placeholder="Nhập lại mật khẩu"
              />
            </div>

            <div class="d-flex align-items-center mb-2">
              <input
                class="form-check-input me-2"
                type="checkbox"
                id="agreeTerms"
                v-model="form.agree"
              />
              <label class="form-check-label small" for="agreeTerms">
                Tôi đồng ý với điều khoản sử dụng và quy tắc cộng đồng.
              </label>
            </div>

            <div v-if="error" class="text-danger small mb-2">
              {{ error }}
            </div>

            <button type="submit" class="btn btn-primary w-100 btn-main mb-2">
              Tạo tài khoản
            </button>
          </form>

          <div class="divider">
            <span>hoặc</span>
          </div>

          <div class="social-row">
            <button
              type="button"
              class="social-icon-btn social-google"
              @click="signupWithGoogle"
              aria-label="Đăng ký với Google"
            >
              <span class="bi bi-google"></span>
            </button>

            <button
              type="button"
              class="social-icon-btn social-facebook"
              @click="signupWithFacebook"
              aria-label="Đăng ký với Facebook"
            >
              <span class="bi bi-facebook"></span>
            </button>
          </div>

          <div class="signup-row">
            <span>Đã có tài khoản?</span>
            <button type="button" class="signup-link" @click="goLogin">
              Đăng nhập
            </button>
          </div>
        </div>

        <p class="copyright small">
          © {{ new Date().getFullYear() }} Convox. All rights reserved.
        </p>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'RegisterView',
  data() {
    return {
      error: '',
      form: {
        userName: '',
        fullName: '',
        password: '',
        confirmPassword: '',
        email: '',
        agree: false,
      },
    }
  },
  methods: {
    submitRegister() {
      this.error = ''

      if (!this.form.fullName || !this.form.email || !this.form.userName || !this.form.password || !this.form.confirmPassword) {
        this.error = 'Vui lòng điền đầy đủ tất cả các trường.'
        return
      }

      if (this.form.password !== this.form.confirmPassword) {
        this.error = 'Mật khẩu nhập lại không khớp.'
        return
      }

      if (!this.form.agree) {
        this.error = 'Bạn cần đồng ý với điều khoản trước khi tiếp tục.'
        return
      }

      const payload = {
        userName: this.form.userName.trim(),
        fullName: this.form.fullName.trim(),
        password: this.form.password,
        email: this.form.email.trim(),
      }

      console.log('Register payload:', payload)
      this.$router.push('/login')
    },
    signupWithGoogle() {
      console.log('Signup with Google')
    },
    signupWithFacebook() {
      console.log('Signup with Facebook')
    },
    goBack() {
      if (window.history.length > 1) {
        this.$router.back()
      } else {
        this.$router.push('/')
      }
    },
    goLogin() {
      this.$router.push('/login')
    },
  },
}
</script>

<style scoped>
.register-page {
  min-height: 100vh;
  background: #020617;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 16px;
  color: #e5e7eb;
}

.register-shell {
  width: 100%;
  max-width: 960px;
  display: flex;
  gap: 0;
  border-radius: 18px;
  overflow: hidden;
  box-shadow: 0 24px 60px rgba(0, 0, 0, 0.85);
  border: 1px solid #111827;
}

.register-side {
  flex: 1;
  background: #111827;
  padding: 20px 22px 16px;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
}

.brand-title {
  font-size: 24px;
  font-weight: 700;
  color: #6366f1;
  margin-bottom: 8px;
}

.brand-subtitle {
  font-size: 14px;
  color: #9ca3af;
  max-width: 280px;
}

.fun-tag {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  margin-top: 12px;
  padding: 4px 10px;
  border-radius: 999px;
  border: 1px solid #1f2937;
  background-color: #020617;
  font-size: 11px;
  color: #e5e7eb;
}

.dot-online {
  width: 6px;
  height: 6px;
  border-radius: 999px;
  background-color: #22c55e;
}

.fun-chat-card {
  margin-top: 14px;
  border-radius: 12px;
  background-color: #020617;
  border: 1px solid #1f2937;
  padding: 10px 12px;
  font-size: 12px;
  color: #e5e7eb;
}

.fun-chat-header {
  display: flex;
  align-items: center;
  gap: 6px;
  margin-bottom: 8px;
}

.room-hash {
  color: #6b7280;
}

.room-name {
  font-weight: 600;
}

.room-pill {
  margin-left: auto;
  font-size: 10px;
  padding: 2px 6px;
  border-radius: 999px;
  background-color: #111827;
  color: #9ca3af;
}

.fun-chat-body {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.fun-msg-name {
  display: flex;
  align-items: baseline;
  gap: 6px;
}

.fun-msg-name .name {
  font-weight: 600;
}

.fun-msg-name .time {
  font-size: 11px;
  color: #6b7280;
}

.fun-msg-text {
  color: #d1d5db;
}

.fun-list {
  list-style: none;
  padding: 0;
  margin: 14px 0 0;
  font-size: 12px;
  color: #9ca3af;
}

.fun-list li + li {
  margin-top: 4px;
}

.brand-note {
  font-size: 12px;
  color: #9ca3af;
  border-top: 1px solid #1f2937;
  padding-top: 8px;
}

.register-main {
  flex: 1;
  background-color: #020617;
  padding: 16px 20px 12px;
  display: flex;
  flex-direction: column;
}

.back-button {
  align-self: flex-start;
  display: inline-flex;
  align-items: center;
  gap: 6px;
  font-size: 13px;
  border-radius: 999px;
  border: 1px solid #4b5563;
  background: transparent;
  color: #e5e7eb;
  padding: 4px 10px;
  cursor: pointer;
  margin-bottom: 10px;
}

.back-button:hover {
  border-color: #6366f1;
  color: #ffffff;
}

.back-icon {
  font-size: 14px;
}

.register-card {
  width: 100%;
  background-color: #0b1120;
  border-radius: 16px;
  border: 1px solid #111827;
  color: #ffffff;
  padding: 16px 16px 14px;
}

.register-header h3 {
  font-size: 20px;
}

.register-header p {
  color: #9ca3af;
}

.row-2col {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 10px;
  margin-bottom: 10px;
}

.field-group {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.register-input {
  background-color: #020617;
  border-radius: 8px;
  border: 1px solid #1f2937;
  color: #ffffff;
  font-size: 14px;
  padding: 8px 10px;
}

.register-input::placeholder {
  color: #6b7280;
}

.register-input:focus {
  background-color: #020617;
  border-color: #6366f1;
  color: #ffffff;
  box-shadow: 0 0 0 1px #6366f1;
  outline: none;
}

.btn-main {
  background-color: #6366f1;
  border-color: #6366f1;
  color: #ffffff;
  font-weight: 600;
  border-radius: 999px;
  padding: 8px 0;
}

.btn-main:hover {
  background-color: #4f46e5;
  border-color: #4f46e5;
  color: #ffffff;
}

.divider {
  display: flex;
  align-items: center;
  gap: 12px;
  font-size: 11px;
  color: #9ca3af;
  margin: 8px 0 10px;
}

.divider::before,
.divider::after {
  content: '';
  flex: 1;
  height: 1px;
  background-color: #1f2937;
}

.social-row {
  display: flex;
  justify-content: center;
  gap: 10px;
  margin-top: 4px;
}

.social-icon-btn {
  width: 38px;
  height: 38px;
  border-radius: 999px;
  border: none;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  font-size: 18px;
  cursor: pointer;
  transition: transform 0.12s ease, box-shadow 0.12s ease, filter 0.12s ease;
}

.social-google {
  background-color: #ffffff;
  color: #4285f4;
  box-shadow: 0 0 0 1px #e5e7eb;
}

.social-facebook {
  background-color: #1877f2;
  color: #ffffff;
  box-shadow: 0 0 0 1px #1d4ed8;
}

.social-icon-btn:hover {
  transform: translateY(-1px);
  filter: brightness(1.05);
  box-shadow: 0 0 0 1px #6366f1, 0 8px 18px rgba(0, 0, 0, 0.55);
}

.social-icon-btn:active {
  transform: translateY(0);
  box-shadow: 0 0 0 1px #4b5563, 0 4px 10px rgba(0, 0, 0, 0.6);
}

.signup-row {
  margin-top: 8px;
  text-align: center;
  font-size: 14px;
  color: #9ca3af;
}

.signup-link {
  background: none;
  border: none;
  padding: 0 0 0 4px;
  cursor: pointer;
  color: #6366f1;
}

.signup-link:hover {
  text-decoration: underline;
  color: #818cf8;
}

.form-check-input {
  background-color: #020617;
  border-color: #4b5563;
}

.form-check-input:checked {
  background-color: #6366f1;
  border-color: #6366f1;
}

.form-check-label {
  color: #9ca3af;
}

.text-danger {
  color: #f87171 !important;
}

.copyright {
  margin-top: auto;
  text-align: center;
  color: #6b7280;
  font-size: 11px;
}

.typing-dots {
  display: inline-flex;
  gap: 3px;
  margin-top: 2px;
}

.typing-dots span {
  width: 4px;
  height: 4px;
  border-radius: 999px;
  background-color: #6b7280;
  animation: typing-bounce 1s infinite ease-in-out;
}

.typing-dots span:nth-child(2) {
  animation-delay: 0.16s;
}

.typing-dots span:nth-child(3) {
  animation-delay: 0.32s;
}

@keyframes typing-bounce {
  0%, 80%, 100% {
    transform: translateY(0);
    opacity: 0.4;
  }
  40% {
    transform: translateY(-3px);
    opacity: 1;
  }
}

@media (max-width: 768px) {
  .register-shell {
    max-width: 480px;
  }
  .register-side {
    display: none !important;
  }
  .register-main {
    padding: 18px 16px 12px;
  }
  .row-2col {
    grid-template-columns: minmax(0, 1fr);
  }
}
</style>
