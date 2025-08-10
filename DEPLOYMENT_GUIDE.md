# Deployment Guide for ShiftSchedularApplication

This guide provides step-by-step instructions for deploying your .NET Core MVC application to Render.com with either Azure SQL Database or AWS RDS.

## Prerequisites

- GitHub repository with your application code
- Render.com account
- Azure SQL Database **OR** AWS RDS instance
- OAuth credentials for Google and Facebook authentication

## Database Setup

### Option 1: Azure SQL Database

#### 1. Create Azure SQL Database
- Go to [Azure Portal](https://portal.azure.com)
- Create a new SQL Database
- Choose **SQL authentication** (recommended for development)
- Set workload environment to **Development**
- Use **Standard-series (Gen5), 2 vCores**
- Choose **Locally-redundant backup storage**
- Set **Public endpoint** for connectivity
- Use **Default connection policy**
- Keep **TLS 1.2** for security

#### 2. Configure Firewall
- **Allow Azure services**: Yes
- **Add current client IP**: Yes (for setup/testing)

#### 3. Create SQL Login
- Go to SQL Server → Security → Logins
- Create new login with SQL authentication
- Username: `shiftschedular_user`
- Strong password (16+ characters, mixed case, numbers, symbols)

### Option 2: AWS RDS (Recommended for AWS Users)

#### 1. Create RDS Instance
- Go to [AWS RDS Console](https://console.aws.amazon.com/rds/)
- Click "Create database"
- Choose **Standard create**
- Engine type: **Microsoft SQL Server**
- Version: **SQL Server 2022 Express** (free tier) or **SQL Server 2022 Standard**
- Template: **Free tier** (if eligible) or **Production**

#### 2. Database Configuration
- **DB instance identifier**: `shiftschedular-db`
- **Master username**: `admin` (or custom username)
- **Master password**: Strong password (12+ characters)
- **Instance size**: **db.t3.micro** (free tier) or **db.t3.small**

#### 3. Storage Configuration
- **Storage type**: **General Purpose SSD (gp2)**
- **Allocated storage**: **20 GB** (minimum)
- **Storage autoscaling**: **Enable** (recommended)
- **Maximum storage threshold**: **100 GB**

#### 4. Connectivity Configuration
- **Public access**: **Yes** (required for Render)
- **VPC security group**: Create new or use existing
- **Availability Zone**: **No preference**
- **Database port**: **1433** (default)

#### 5. Security Configuration
- **Encryption**: **Enable encryption**
- **Backup retention**: **7 days** (free tier) or **30 days**
- **Performance Insights**: **Enable** (recommended)

#### 6. Create Database
- Click "Create database"
- Wait for status to become "Available"

## Render.com Deployment

### 1. Create New Web Service
- Go to [Render Dashboard](https://dashboard.render.com/)
- Click "New +" → "Web Service"
- Connect your GitHub repository
- Choose the repository: `ShiftSchedularApplication`

### 2. Configure Service
- **Name**: `shiftschedular-app`
- **Environment**: `C#`
- **Build Command**: `dotnet publish -c Release -o out`
- **Start Command**: `dotnet out/ShiftSchedularApplication.dll`
- **Plan**: Choose appropriate plan (Free tier available)

### 3. Environment Variables
Set these environment variables in Render:

#### Database Connection String
Choose one based on your database provider:

**For Azure SQL:**
```
ASPNETCORE_CONNECTION_STRING = Server=your-azure-server.database.windows.net;Database=ShiftSchedular-DB;User Id=shiftschedular_user;Password=your-password;TrustServerCertificate=true;MultipleActiveResultSets=true
```

**For AWS RDS:**
```
ASPNETCORE_CONNECTION_STRING = Server=your-rds-endpoint.region.rds.amazonaws.com,1433;Database=shiftschedular-db;User Id=admin;Password=your-password;TrustServerCertificate=true;MultipleActiveResultSets=true
```

#### OAuth Configuration
```
GOOGLE_CLIENT_ID = your-google-client-id
GOOGLE_CLIENT_SECRET = your-google-client-secret
FACEBOOK_APP_ID = your-facebook-app-id
FACEBOOK_APP_SECRET = your-facebook-app-secret
```

#### Application Configuration
```
ASPNETCORE_ENVIRONMENT = Production
ASPNETCORE_URLS = http://0.0.0.0:10000
```

### 4. Deploy
- Click "Create Web Service"
- Render will automatically build and deploy your application
- Monitor the build logs for any errors

## Post-Deployment Setup

### 1. Database Migration
After successful deployment, run database migrations:

```bash
# Connect to your database and run:
dotnet ef database update
```

### 2. Test OAuth
- Test Google and Facebook login on your live site
- Ensure redirect URIs are configured correctly
- Update OAuth providers with your Render domain

### 3. Security Considerations
- Remove your personal IP from firewall rules
- Add Render's IP ranges to database firewall
- Keep Azure services access enabled (if using Azure)
- Ensure strong passwords for database users

## Troubleshooting

### Common Issues

1. **Connection String Errors**
   - Verify server name and port
   - Check username and password
   - Ensure firewall allows connections

2. **OAuth Issues**
   - Verify redirect URIs in OAuth provider settings
   - Check environment variables are set correctly
   - Ensure HTTPS is enabled

3. **Build Failures**
   - Check build logs in Render
   - Verify all dependencies are in `.csproj`
   - Ensure code compiles locally

### Support Resources

- [Render Documentation](https://render.com/docs)
- [AWS RDS Documentation](https://docs.aws.amazon.com/rds/)
- [Azure SQL Documentation](https://docs.microsoft.com/en-us/azure/azure-sql/)
- [ASP.NET Core Deployment](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/)

## Cost Optimization

### AWS RDS
- **Free tier**: 750 hours/month of db.t3.micro
- **Production**: db.t3.small (~$15/month)
- **Storage**: $0.10/GB/month

### Azure SQL
- **Development**: ~$5/month for 2 vCores
- **Production**: ~$30/month for 2 vCores
- **Storage**: Included in compute cost

### Render
- **Free tier**: Available for web services
- **Paid plans**: Starting at $7/month

Choose the combination that fits your budget and requirements!
