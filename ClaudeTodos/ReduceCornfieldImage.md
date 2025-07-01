# Reduce Cornfield Image - Implementation Plan

## Current State Analysis

### Image Properties
- **Current File**: `src/apps/blazor/client/wwwroot/cornfield.png`
- **File Size**: 3.1 MB
- **Dimensions**: 1536 x 1024 pixels
- **Format**: PNG image data, 8-bit/color RGB, non-interlaced

### Current Usage
The cornfield.png image is used in:
1. **Blazor Client Pages**
   - Home.razor
   - Error/AuthError.razor
   - Auth/AcceptInvitation.razor
2. **CSS Files**
   - dark-panel.css
   - loading.css
3. **AD B2C Templates** (via Azure Blob Storage URL)
   - unified.html
   - selfasserted.html
   - emailverification.html

## Image Optimization Plan

### Step 1: Create Optimized Version ✅ COMPLETED
1. **Install Image Processing Tools**
   ```bash
   # Install ImageMagick (if not already installed)
   sudo apt-get update
   sudo apt-get install imagemagick
   
   # Or alternatively, install pngquant for PNG optimization
   sudo apt-get install pngquant
   ```

2. **Create Reduced Version**
   ```bash
   # Navigate to the image directory
   cd src/apps/blazor/client/wwwroot/
   
   # Option A: Using ImageMagick
   # Resize to 1024x683 (maintains aspect ratio), optimize compression
   convert cornfield.png -resize 1024x683 -quality 85 -strip cornfieldreduced.png
   
   # Option B: Using pngquant for better PNG compression
   # First resize, then compress
   convert cornfield.png -resize 1024x683 cornfield_temp.png
   pngquant --quality=85-95 --strip cornfield_temp.png -o cornfieldreduced.png
   rm cornfield_temp.png
   
   # Option C: Progressive loading with multiple resolutions
   convert cornfield.png -resize 768x512 -quality 85 -strip cornfield-medium.png
   convert cornfield.png -resize 384x256 -quality 85 -strip cornfield-small.png
   ```

3. **Further Optimization**
   ```bash
   # Use optipng for lossless compression
   sudo apt-get install optipng
   optipng -o7 cornfieldreduced.png
   
   # Or use pngcrush
   sudo apt-get install pngcrush
   pngcrush -reduce -brute cornfieldreduced.png cornfieldreduced_optimized.png
   mv cornfieldreduced_optimized.png cornfieldreduced.png
   ```

### Step 2: Convert to WebP Format (Optional but Recommended) ✅ COMPLETED
```bash
# Install WebP tools
sudo apt-get install webp

# Convert to WebP for better compression
cwebp -q 85 cornfieldreduced.png -o cornfieldreduced.webp
```

### Step 3: Update Blazor Application References ✅ COMPLETED

1. **Update CSS Files**
   - Modify `dark-panel.css` to use `cornfieldreduced.png`
   - Update `loading.css` to use `cornfieldreduced.png`
   - Consider implementing CSS media queries for responsive images

2. **Update Razor Components**
   - Update Home.razor
   - Update Error/AuthError.razor
   - Update Auth/AcceptInvitation.razor

3. **Implement Picture Element for Browser Support**
   ```html
   <picture>
     <source srcset="cornfieldreduced.webp" type="image/webp">
     <img src="cornfieldreduced.png" alt="Cornfield background">
   </picture>
   ```

### Step 4: Upload to Azure Blob Storage

1. **Upload Optimized Image**
   ```bash
   # Using Azure CLI
   az storage blob upload \
     --account-name ohdb2ctemplates \
     --container-name ohdb2c-templates \
     --name cornfieldreduced.png \
     --file cornfieldreduced.png \
     --content-type "image/png"
   
   # Upload WebP version if created
   az storage blob upload \
     --account-name ohdb2ctemplates \
     --container-name ohdb2c-templates \
     --name cornfieldreduced.webp \
     --file cornfieldreduced.webp \
     --content-type "image/webp"
   ```

2. **Set Cache Headers**
   ```bash
   # Set long cache duration for better performance
   az storage blob update \
     --account-name ohdb2ctemplates \
     --container-name ohdb2c-templates \
     --name cornfieldreduced.png \
     --content-cache-control "public, max-age=31536000"
   ```

### Step 5: Update AD B2C Templates

1. **Update HTML Templates** ✅ COMPLETED
   - Modify `unified.html` to reference the new image URL
   - Update `selfasserted.html` 
   - Update `emailverification.html`

2. **Implement Preload and Lazy Loading** ✅ COMPLETED
   ```html
   <!-- Add to <head> -->
   <link rel="preload" href="https://ohdb2ctemplates.blob.core.windows.net/ohdb2c-templates/cornfieldreduced.png" as="image">
   
   <!-- Or for responsive images -->
   <link rel="preload" href="https://ohdb2ctemplates.blob.core.windows.net/ohdb2c-templates/cornfield-small.png" as="image" media="(max-width: 768px)">
   <link rel="preload" href="https://ohdb2ctemplates.blob.core.windows.net/ohdb2c-templates/cornfieldreduced.png" as="image" media="(min-width: 769px)">
   ```

### Step 6: Additional Performance Optimizations

1. **Implement CSS Background Loading Strategy**
   ```css
   /* Progressive enhancement approach */
   body {
     background-color: var(--background-color);
     background-image: url('cornfield-small.png');
   }
   
   @media (min-width: 768px) {
     body {
       background-image: url('cornfieldreduced.png');
     }
   }
   ```

2. **Consider Using CSS Blur Effect**
   ```css
   /* Load smaller image with blur for faster initial load */
   body::before {
     content: '';
     position: fixed;
     top: 0;
     left: 0;
     width: 100%;
     height: 100%;
     background-image: url('cornfield-small.png');
     background-size: cover;
     filter: blur(5px);
     z-index: -1;
   }
   ```

3. **Enable CDN Caching**
   - Configure Azure CDN for the blob storage container
   - Set appropriate cache headers
   - Enable compression at CDN level

### Step 7: Testing and Validation

1. **Performance Testing**
   ```bash
   # Test load times
   curl -o /dev/null -s -w 'Total: %{time_total}s\n' https://ohdb2ctemplates.blob.core.windows.net/ohdb2c-templates/cornfieldreduced.png
   ```

2. **Browser Testing**
   - Test in multiple browsers (Chrome, Firefox, Safari, Edge)
   - Verify responsive loading on different screen sizes
   - Check WebP fallback for unsupported browsers

3. **Lighthouse Audit**
   - Run Lighthouse performance audit before and after changes
   - Verify improvements in:
     - First Contentful Paint (FCP)
     - Largest Contentful Paint (LCP)
     - Total Blocking Time (TBT)

## Expected Results

### Size Reduction
- Original: 3.1 MB (1536x1024)
- Optimized PNG: ~800 KB - 1.2 MB (1024x683)
- WebP version: ~400-600 KB
- Total reduction: 60-80%

### Performance Improvements
- Faster initial page load
- Reduced bandwidth usage
- Better mobile performance
- Improved Core Web Vitals scores

## Rollback Plan

If issues arise:
1. Keep original cornfield.png file unchanged
2. Revert B2C template changes by updating URLs back to original
3. Revert Blazor application references
4. Remove new files from Azure Blob Storage

## Implementation Timeline

1. **Phase 1** (Local Development)
   - Create optimized images
   - Update Blazor application
   - Test locally

2. **Phase 2** (Azure Deployment)
   - Upload to blob storage
   - Update B2C templates
   - Test in staging environment

3. **Phase 3** (Production)
   - Deploy to production
   - Monitor performance metrics
   - Gather user feedback