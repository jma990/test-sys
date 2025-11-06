#!/bin/bash
# Startup script

# Step 1: Run the app in the background
export USE_NGROK=true
echo "🚀 Starting .NET application..."
dotnet run &
APP_PID=$!

# Give it a few seconds to fully start
sleep 5

# Step 2: Start ngrok (random public URL)
echo "🌐 Starting ngrok tunnel..."
ngrok http 5282 > ngrok.log &

# Give ngrok a few seconds to start and log the URL
sleep 5

# Step 3: Extract and display the public URL from ngrok’s log
PUBLIC_URL=$(grep -o 'https://[a-z0-9-]*\.ngrok-free\.app' ngrok.log | head -n 1)

if [ -n "$PUBLIC_URL" ]; then
  echo "✅ Your CMS is live at: $PUBLIC_URL"
else
  echo "⚠️ Could not detect ngrok URL. Check ngrok.log for details."
fi

# Step 4: Keep script running until you stop it manually (Ctrl+C)
wait

# Cleanup
kill $APP_PID

