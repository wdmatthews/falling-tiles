const express = require('express');
const app = express();
const formidableMiddleware = require('express-formidable');
const { MongoClient } = require('mongodb');
const mongoURL = `mongodb://localhost:27017/falling_tiles`;
const bcrypt = require('bcrypt');

app.disable('x-powered-by');
app.use(express.static('public'));
app.use(formidableMiddleware());

app.get('/', (req, res) => {
  res.sendFile(__dirname + '/views/index.html');
});

async function getUser(username, password) {
  let client = null;
  let user = null;
  
  try {
    client = await MongoClient.connect(mongoURL, { useUnifiedTopology: true });
    const users = client.db('falling_tiles').collection('users');
    user = await users.findOne({ username });
    
    if (user) {
      const passwordMatches = await bcrypt.compare(password, user.password);
      if (!passwordMatches) user = null;
    }
  } finally {
    await client.close();
  }
  
  return user;
}

async function registerUser(username, password) {
  let client = null;
  let success = false;
  
  try {
    client = await MongoClient.connect(mongoURL, { useUnifiedTopology: true });
    const users = client.db('falling_tiles').collection('users');
    const hashedPassword = await bcrypt.hash(password, 10);
    await users.insertOne({ username, password: hashedPassword, save: '' });
    success = true;
  } finally {
    await client.close();
  }
  
  return success;
}

async function saveData(username, save) {
  let client = null;
  let success = false;
  
  try {
    client = await MongoClient.connect(mongoURL, { useUnifiedTopology: true });
    const users = client.db('falling_tiles').collection('users');
    await users.updateOne({ username }, { $set: { save } });
    success = true;
  } finally {
    await client.close();
  }
  
  return success;
}

app.post('/login', async (req, res) => {
  if (typeof req.fields.username != 'string' || typeof req.fields.password != 'string') {
    res.send('fail');
    return;
  }
  
  const user = await getUser(req.fields.username, req.fields.password);
  res.send(user ? 'success' : 'fail');
});

app.post('/register', async (req, res) => {
  if (typeof req.fields.username != 'string' || typeof req.fields.password != 'string') {
    res.send('fail');
    return;
  }
  
  const username = req.fields.username;
  const password = req.fields.password;
  const user = await getUser(username, password);
  
  if (user) {
    res.send('fail');
    return;
  }
  
  const success = await registerUser(username, password);
  res.send(success ? 'success' : 'fail');
});

app.post('/load', async (req, res) => {
  if (typeof req.fields.username != 'string' || typeof req.fields.password != 'string') {
    res.send('fail');
    return;
  }
  
  const user = await getUser(req.fields.username, req.fields.password);
  res.send(user ? user.save : 'fail');
});

app.post('/save', async (req, res) => {
  if (typeof req.fields.username != 'string' || typeof req.fields.password != 'string' || typeof req.fields.save != 'string') {
    res.send('fail');
    return;
  }
  
  const username = req.fields.username;
  const password = req.fields.password;
  const save = req.fields.save;
  const user = await getUser(username, password);
  
  if (!user) {
    res.send('fail');
    return;
  }
  
  const success = await saveData(username, save);
  res.send(success ? 'success' : 'fail');
});

app.listen(3000, () => console.log(`App listening at http://localhost:3000`));