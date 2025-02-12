 
 🌦️ Weather App Features

 🔹 Authentication & User Management  
- User authentication using Supabase  
- Login credentials stored using cookies for session persistence.  
- Logout functionality to clear authentication state.  

📍 User Preferences & Favorites
- Users can save favorite cities for quick access.  
- Home city selection for personalized weather updates.  
- Preferences stored in MongoDB.  

 ⛅ Weather Data & Forecasts
- Real-time weather data from OpenWeather API.  
- Current weather conditions (temperature, humidity, wind speed, etc.).  
- Hourly and daily forecasts.  
- UV index, air quality, and precipitation probability.
- Only Authenticated user will get the full details of the Weather data
- Non Authenticated user will get only less data and can search only 5 times without login
- 5 Days forecast is only visible for the authneticated user

 💬 AI-Powered Weather Chat 
- Chatbot powered by Gemini AI and WeatherChatService.  
- Provides AI-generated weather insights and answers.  
- Dual API connection: 
  - `WeatherChatController`: Provides weather + AI-generated responses.  
  - `GeminiChatController`: Provides AI-only responses.  
- User-specific chat history with the ability to delete chat history.  

 📩 Weather Alerts & Notifications  
- Email notifications for rain alerts.  
- Notifications sent via backend API.  

 🔗 Backend Services & APIs 
- WeatherController: Fetches weather data.  
- FavoritesController: Manages favorite cities.  
- AuthController: Handles authentication.  
- WeatherChatService: Integrates OpenWeather API & Gemini AI.  

🔥 Performance & Optimization  
- Caching of weather data for improved performance.  
- Efficient API usage to minimize requests.  

 🏗️ Tech Stack  
- Blazor WebAssembly (Frontend).  
- ASP.NET Core 8 (Backend).  
- Supabase (Authentication).  
- MongoDB (User preferences storage).  
- OpenWeather API (Weather data).  
- Gemini AI (Chatbot AI).  


