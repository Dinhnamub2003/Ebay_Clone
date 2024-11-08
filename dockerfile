FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Sao chép các file .csproj và khôi phục các package (sử dụng caching để tăng tốc độ build)
COPY *.sln ./
COPY Project.Bussiness/*.csproj ./Project.Bussiness/
COPY Project.Common/*.csproj ./Project.Common/
COPY Project.Data/*.csproj ./Project.Data/
COPY Project.Model/*.csproj ./Project.Model/
COPY Project.RazorWeb/*.csproj ./Project.RazorWeb/
COPY Project.Servie/*.csproj ./Project.Servie/
COPY Project.RazorWeb/appsettings.json ./appsettings.json
RUN dotnet restore

# Sao chép tất cả các file còn lại và build ứng dụng
COPY . ./
RUN dotnet publish Project.RazorWeb/Project.RazorWeb.csproj -c Release -o out

# Stage 2: Node.js environment for Node modules
FROM node:20 AS node-env
WORKDIR /app

# Tạo thư mục node_modules nếu chưa có
RUN mkdir -p Project.RazorWeb/node_modules

# Sao chép thư mục node_modules nếu đã có sẵn
COPY Project.RazorWeb/node_modules ./Project.RazorWeb/node_modules

# Bước cuối: chạy ứng dụng trên image .NET Runtime (nhẹ hơn SDK)
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .

# Copy thư mục node_modules từ Node.js stage
COPY --from=node-env /app/Project.RazorWeb/node_modules ./Project.RazorWeb/node_modules


# Thiết lập biến môi trường nếu cần (tùy chọn)
# ENV ASPNETCORE_ENVIRONMENT=Development

# Mở cổng 80 để truy cập vào ứng dụng
EXPOSE 80

# Khởi động ứng dụng
ENTRYPOINT ["dotnet", "Project.RazorWeb.dll"]
