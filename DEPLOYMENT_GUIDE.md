# BinDaddy Backend - Deployment Guide

This guide walks you through deploying the BinDaddy ASP.NET Core backend to Railway.

## Prerequisites

- GitHub account with the repository pushed
- Railway account (https://railway.app)
- Stripe account (for payment keys)

## Step 1: Push Code to GitHub

```bash
cd bindaddy-backend
git init
git add .
git commit -m "Initial commit: ASP.NET Core backend"
git branch -M main
git remote add origin https://github.com/YOUR_USERNAME/bindaddy-backend.git
git push -u origin main
```

## Step 2: Create Railway Project

1. Go to https://railway.app
2. Click "New Project"
3. Select "Deploy from GitHub"
4. Authorize Railway to access your GitHub account
5. Select the `bindaddy-backend` repository
6. Click "Deploy"

Railway will automatically:
- Detect the .NET project
- Build the Docker image
- Deploy the application

## Step 3: Add MySQL Database

1. In Railway dashboard, click "Add Service"
2. Select "MySQL"
3. Railway will create a MySQL instance
4. Copy the connection string from the MySQL service

## Step 4: Configure Environment Variables

1. Go to your Railway project
2. Click on the backend service
3. Go to "Variables" tab
4. Add the following variables:

```
DATABASE_URL=mysql://root:password@mysql:3306/bindaddy
ASPNETCORE_ENVIRONMENT=Production
STRIPE_SECRET_KEY=sk_test_YOUR_KEY
STRIPE_PUBLISHABLE_KEY=pk_test_YOUR_KEY
JWT_SECRET=your-super-secret-jwt-key
PORT=5000
```

### Getting the DATABASE_URL

Railway provides the connection string in the MySQL service. It will look like:

```
mysql://root:PASSWORD@HOST:PORT/DATABASE
```

Convert it to the format needed:

```
Server=HOST;Port=PORT;Database=DATABASE;User=root;Password=PASSWORD;
```

## Step 5: Configure Application

Update `appsettings.Production.json` to use environment variables:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "${DATABASE_URL}"
  },
  "Stripe": {
    "SecretKey": "${STRIPE_SECRET_KEY}",
    "PublishableKey": "${STRIPE_PUBLISHABLE_KEY}"
  },
  "Jwt": {
    "Secret": "${JWT_SECRET}"
  }
}
```

## Step 6: Verify Deployment

1. Wait for Railway to finish building and deploying
2. Click on your backend service
3. Go to the "Deployments" tab
4. Check the deployment logs
5. Once deployed, you'll see a public URL

## Step 7: Test the API

Once deployed, test your API:

```bash
# Get the Railway URL from the dashboard
# Replace RAILWAY_URL with your actual URL

# Test health check
curl https://RAILWAY_URL/health

# Get all products
curl https://RAILWAY_URL/api/products

# Get Swagger documentation
https://RAILWAY_URL/swagger
```

## Step 8: Connect Frontend

Update your Next.js frontend environment variables:

```env
NEXT_PUBLIC_API_URL=https://RAILWAY_URL/api
NEXT_PUBLIC_STRIPE_PUBLISHABLE_KEY=pk_test_YOUR_KEY
```

## Troubleshooting

### Build Fails
- Check the build logs in Railway dashboard
- Ensure all dependencies are specified in `.csproj`
- Verify .NET version is 8.0

### Database Connection Error
- Verify `DATABASE_URL` is correct
- Ensure MySQL service is running
- Check database name matches

### Application Crashes
- Check application logs in Railway dashboard
- Verify all environment variables are set
- Check database migrations ran successfully

### CORS Issues
- Verify frontend URL is allowed in CORS policy
- Update `Program.cs` if needed

## Monitoring

Railway provides built-in monitoring:

1. **Logs**: View real-time application logs
2. **Metrics**: Monitor CPU, memory, and network usage
3. **Deployments**: Track deployment history

## Scaling

To scale your application:

1. Go to Railway dashboard
2. Select your backend service
3. Adjust resource allocation (CPU, Memory)
4. Railway will automatically restart with new resources

## Custom Domain

To use a custom domain:

1. Go to Railway project settings
2. Click "Domains"
3. Add your custom domain
4. Follow DNS configuration instructions

## Continuous Deployment

Railway automatically deploys when you push to GitHub:

1. Push changes to main branch
2. Railway detects the change
3. Builds and deploys automatically
4. Old deployment remains available for rollback

## Rollback

To rollback to a previous deployment:

1. Go to "Deployments" tab
2. Select the previous deployment
3. Click "Rollback"

## Database Backups

Railway provides automatic daily backups. To restore:

1. Go to MySQL service
2. Click "Backups"
3. Select backup date
4. Click "Restore"

## Next Steps

1. ✅ Deploy backend to Railway
2. ✅ Configure environment variables
3. ✅ Test API endpoints
4. ✅ Connect frontend application
5. ✅ Set up monitoring and alerts
6. ✅ Configure custom domain
7. ✅ Set up CI/CD pipeline

## Support

For Railway support: https://railway.app/support
For ASP.NET Core documentation: https://learn.microsoft.com/aspnet/core/

## Security Checklist

- [ ] Change default passwords
- [ ] Use strong JWT secret
- [ ] Enable HTTPS (Railway does this automatically)
- [ ] Restrict CORS to frontend domain
- [ ] Use environment variables for secrets
- [ ] Enable database backups
- [ ] Monitor application logs
- [ ] Set up error tracking (e.g., Sentry)
