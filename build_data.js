const fs = require('fs');
const path = require('path');

const receiptsDir = path.join(__dirname, 'receipts');
const mappingPath = path.join(__dirname, 'store_mapping.json');
const storeMapping = JSON.parse(fs.readFileSync(mappingPath, 'utf-8'));
const files = fs.readdirSync(receiptsDir).filter(f => f.endsWith('.json'));

const records = [];

for (const file of files) {
  const raw = fs.readFileSync(path.join(receiptsDir, file), 'utf-8').replace(/^\uFEFF/, '');
  const receipt = JSON.parse(raw);

  // Skip delivery-confirmation receipts where the full amount was already pre-paid
  if (receipt.prepaidSum != null && receipt.totalSum != null && receipt.prepaidSum >= receipt.totalSum) continue;

  const items = receipt.items || [];
  const hasNamedItems = items.some(i => i.name != null && i.name !== '');
  if (!hasNamedItems) continue;

  for (const item of items) {
    if (item.name == null || item.name === '') continue;
    records.push({
      name: item.name,
      sum: item.sum,
      quantity: item.quantity,
      price: item.price,
      date: receipt.dateTime,
      store: storeMapping[receipt.user] || storeMapping[receipt.retailPlace] || receipt.user || receipt.retailPlace || 'Unknown',
    });
  }
}

const output = 'const RECEIPT_DATA = ' + JSON.stringify(records, null, 2) + ';\n';
fs.writeFileSync(path.join(__dirname, 'data.js'), output, 'utf-8');
console.log(`Processed ${files.length} files, extracted ${records.length} item records.`);
