# Angular 17 Frontend Configuration

## Browser Support

The application supports:
- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)

## Node Version

Required: Node.js 18.0.0 or higher

## TypeScript Version

TypeScript 5.2.0

## Angular Version

Angular 17.0.0

## Build Configuration

### Development
```bash
npm start
```
- No optimization
- Source maps enabled
- Fast rebuild
- Hot module replacement

### Production
```bash
npm run build:prod
```
- Code optimization
- Tree shaking
- Minification
- Production environment variables

## Webpack Configuration

The application uses Angular CLI's default webpack configuration.

## SCSS Support

All styles use SCSS with variables defined in `src/styles/variables.scss`.

## Material Theme

The application uses Angular Material default theme.

To customize:
1. Edit Material theme in component styles
2. Update color variables in `src/styles/variables.scss`

## Assets

Static assets are stored in `src/assets/`

## Environment Configuration

Different configurations for development and production in `src/environments/`

## Performance Optimizations

- Lazy loading for feature modules
- OnPush change detection strategy (where applicable)
- Tree shaking
- Production optimization
- Minification
- AOT compilation
