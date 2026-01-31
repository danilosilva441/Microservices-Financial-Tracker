const fs = require('fs');
const path = require('path');

function checkVueFiles(dir) {
  const files = fs.readdirSync(dir, { withFileTypes: true });
  
  for (const file of files) {
    const fullPath = path.join(dir, file.name);
    
    if (file.isDirectory()) {
      checkVueFiles(fullPath);
    } else if (file.name.endsWith('.vue')) {
      const content = fs.readFileSync(fullPath, 'utf8');
      
      if (!content.includes('<template>') && !content.includes('<script>')) {
        console.error(`❌ Arquivo Vue sem template ou script: ${fullPath}`);
        process.exit(1);
      }
      
      // Verifica se tem o bloco script setup
      if (!content.includes('<script') && !content.includes('<template')) {
        console.error(`❌ Arquivo Vue incompleto: ${fullPath}`);
        process.exit(1);
      }
      
      console.log(`✓ ${fullPath}`);
    }
  }
}

// Verifica a pasta src
checkVueFiles(path.join(__dirname, 'src'));
console.log('✅ Todos os arquivos Vue estão OK!');