import { useEffect, useState } from 'react';

const API_URL = 'http://localhost:5000';

function App() {
  const [customers, setCustomers] = useState([]);
  const [orders, setOrders] = useState([]);
  const [name, setName] = useState('');
  const [email, setEmail] = useState('');
  const [phone, setPhone] = useState('');
  const [address, setAddress] = useState('');

  const loadData = async () => {
    const [customersRes, ordersRes] = await Promise.all([
      fetch(`${API_URL}/api/customers`),
      fetch(`${API_URL}/api/orders`)
    ]);
    setCustomers(await customersRes.json());
    setOrders(await ordersRes.json());
  };

  useEffect(() => {
    loadData();
  }, []);

  const handleCreateCustomer = async (e) => {
    e.preventDefault();
    await fetch(`${API_URL}/api/customers`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ name: name, email, phone, address })
    });
    setName('');
    setEmail('');
    setPhone('');
    setAddress('');
    loadData();
  };

  return (
    <div style={{ fontFamily: 'sans-serif', padding: 24 }}>
      <h1>Sales Assessment</h1>
      <p>Demo CRUD cho Customer và Order</p>

      <form onSubmit={handleCreateCustomer} style={{ marginBottom: 24 }}>
        <input value={name} onChange={(e) => setName(e.target.value)} placeholder="Name" />
        <input value={email} onChange={(e) => setEmail(e.target.value)} placeholder="Email" />
        <input value={phone} onChange={(e) => setPhone(e.target.value)} placeholder="Phone" />
        <input value={address} onChange={(e) => setAddress(e.target.value)} placeholder="Address" />
        <button type="submit">Add Customer</button>
      </form>

      <h2>Customers</h2>
      <ul>
        {customers.map((customer) => (
          <li key={customer.id}>{customer.name} - {customer.email}</li>
        ))}
      </ul>

      <h2>Orders</h2>
      <ul>
        {orders.map((order) => (
          <li key={order.id}>Order #{order.id} - {order.status} - {order.totalAmount}</li>
        ))}
      </ul>
    </div>
  );
}

export default App;
