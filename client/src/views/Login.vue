<template>
  <div class="login-container">
    <div class="login-card">
      <div class="login-header">
        <h1>🎓 Convox</h1>
        <p>Đăng nhập vào hệ thống</p>
      </div>

      <form @submit.prevent="handleLogin" class="login-form">
        <div class="form-group">
          <label for="username">Tên đăng nhập</label>
          <input
            id="username"
            v-model="username"
            type="text"
            placeholder="Nhập tên đăng nhập"
            required
            :disabled="isLoading"
          />
        </div>

        <div class="form-group">
          <label for="password">Mật khẩu</label>
          <input
            id="password"
            v-model="password"
            type="password"
            placeholder="Nhập mật khẩu"
            required
            :disabled="isLoading"
          />
        </div>

        <div class="form-group">
          <label for="apiUrl">API URL</label>
          <input
            id="apiUrl"
            v-model="apiUrl"
            type="text"
            placeholder="https://api.example.com"
            required
            :disabled="isLoading"
          />
        </div>

        <div v-if="errorMessage" class="error-message">
          {{ errorMessage }}
        </div>

        <button type="submit" class="btn-login" :disabled="isLoading">
          {{ isLoading ? "Đang đăng nhập..." : "Đăng Nhập" }}
        </button>
      </form>

      <div class="login-footer">
        <p>
          Chưa có tài khoản?
          <router-link to="/register">Đăng ký ngay</router-link>
        </p>
      </div>
    </div>
  </div>
</template>

<script>
import { ref } from "vue";
import { useRouter } from "vue-router";
import axios from "axios";

export default {
  name: "Login",
  setup() {
    const router = useRouter();

    const username = ref("");
    const password = ref("");
    const apiUrl = ref("http://localhost:5199");
    const errorMessage = ref("");
    const isLoading = ref(false);

    const handleLogin = async () => {
      errorMessage.value = "";
      isLoading.value = true;

      try {
        const response = await axios.post(
          `${apiUrl.value}/api/Authenticate/login`,
          {
            userName: username.value,
            password: password.value,
          }
        );

        if (response.data.isSuccess) {
          const { token, userId, userName } = response.data.data;

          // Save to localStorage
          localStorage.setItem("convox_token", token);
          localStorage.setItem("convox_userId", userId);
          localStorage.setItem("convox_username", userName);
          localStorage.setItem("convox_apiUrl", apiUrl.value);

          // Redirect to test chat
          router.push("/test-chat");
        } else {
          errorMessage.value = response.data.message || "Đăng nhập thất bại";
        }
      } catch (error) {
        console.error("Login error:", error);
        if (error.response) {
          errorMessage.value =
            error.response.data?.message ||
            error.response.data?.errors?.[0] ||
            "Sai tên đăng nhập hoặc mật khẩu";
        } else if (error.request) {
          errorMessage.value =
            "Không thể kết nối đến server. Vui lòng kiểm tra API URL";
        } else {
          errorMessage.value = "Đã có lỗi xảy ra. Vui lòng thử lại";
        }
      } finally {
        isLoading.value = false;
      }
    };

    return {
      username,
      password,
      apiUrl,
      errorMessage,
      isLoading,
      handleLogin,
    };
  },
};
</script>

<style scoped>
.login-container {
  min-height: 100vh;
  display: flex;
  justify-content: center;
  align-items: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 20px;
}

.login-card {
  background: white;
  border-radius: 12px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
  padding: 40px;
  width: 100%;
  max-width: 450px;
}

.login-header {
  text-align: center;
  margin-bottom: 30px;
}

.login-header h1 {
  font-size: 36px;
  margin-bottom: 10px;
  color: #333;
}

.login-header p {
  color: #666;
  font-size: 16px;
}

.login-form {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.form-group label {
  font-weight: 600;
  color: #333;
  font-size: 14px;
}

.form-group input {
  padding: 12px 16px;
  border: 1px solid #ddd;
  border-radius: 8px;
  font-size: 15px;
  transition: border-color 0.3s;
}

.form-group input:focus {
  outline: none;
  border-color: #667eea;
}

.form-group input:disabled {
  background-color: #f5f5f5;
  cursor: not-allowed;
}

.error-message {
  padding: 12px;
  background-color: #fee;
  border: 1px solid #fcc;
  border-radius: 6px;
  color: #c33;
  font-size: 14px;
  text-align: center;
}

.btn-login {
  padding: 14px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 8px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: transform 0.2s, box-shadow 0.2s;
}

.btn-login:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(102, 126, 234, 0.4);
}

.btn-login:disabled {
  background: #ccc;
  cursor: not-allowed;
  transform: none;
}

.login-footer {
  margin-top: 25px;
  text-align: center;
  color: #666;
  font-size: 14px;
}

.login-footer a {
  color: #667eea;
  text-decoration: none;
  font-weight: 600;
}

.login-footer a:hover {
  text-decoration: underline;
}
</style>
