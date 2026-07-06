import { useEffect, useMemo, useState } from 'react';
import { createCustomer, createOrder, deleteCustomer, deleteOrder, getCustomers, getOrders, login, logout, updateCustomer, updateOrder } from './api';

function App() {
  const [token, setToken] = useState(localStorage.getItem('token') || '');
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [customers, setCustomers] = useState([]);
  const [orders, setOrders] = useState([]);
  const [customerForm, setCustomerForm] = useState({ id: 0, name: '', email: '', phone: '', address: '' });
  const [orderForm, setOrderForm] = useState({ id: 0, customerId: 1, orderDate: '', totalAmount: '', status: 'Pending' });
  const [message, setMessage] = useState('');

  const isLoggedIn = useMemo(() => Boolean(token), [token]);

  const loadData = async () => {
    try {
      const [customerData, orderData] = await Promise.all([getCustomers(), getOrders()]);
      setCustomers(customerData);
      setOrders(orderData);
    } catch (error) {
      setMessage(error.message);
    }
  };

  useEffect(() => {
    if (token) {
      loadData();
    }
  }, [token]);

  const handleLogin = async (e) => {
    e.preventDefault();
    try {
      const data = await login(username, password);
      setToken(data.token);
      setMessage('Đăng nhập thành công');
    } catch (error) {
      setMessage(error.message);
    }
  };

  const handleLogout = () => {
    logout();
    setToken('');
    setMessage('Đã đăng xuất');
  };

  const handleCustomerSubmit = async (e) => {
    e.preventDefault();
    try {
      if (customerForm.id) {
        await updateCustomer(customerForm.id, customerForm);
      } else {
        await createCustomer(customerForm);
      }
      setCustomerForm({ id: 0, name: '', email: '', phone: '', address: '' });
      await loadData();
      setMessage('Customer saved');
    } catch (error) {
      setMessage(error.message);
    }
  };

  const handleOrderSubmit = async (e) => {
    e.preventDefault();
    try {
      const payload = {
        ...orderForm,
        orderDate: orderForm.orderDate || new Date().toISOString(),
        totalAmount: Number(orderForm.totalAmount)
      };
      if (orderForm.id) {
        await updateOrder(orderForm.id, payload);
      } else {
        await createOrder(payload);
      }
      setOrderForm({ id: 0, customerId: 1, orderDate: '', totalAmount: '', status: 'Pending' });
      await loadData();
      setMessage('Order saved');
    } catch (error) {
      setMessage(error.message);
    }
  };

  const handleDeleteCustomer = async (id) => {
    await deleteCustomer(id);
    await loadData();
    setMessage('Customer deleted');
  };

  const handleDeleteOrder = async (id) => {
    await deleteOrder(id);
    await loadData();
    setMessage('Order deleted');
  };

  return (
    <div style={{ fontFamily: 'sans-serif', padding: 24, maxWidth: 1100, margin: '0 auto' }}>
      <h1>Sales Assessment</h1>
      <p>Quản lý khách hàng và đơn hàng</p>
      {message && <div style={{ background: '#f5f5f5', padding: 10, marginBottom: 12 }}>{message}</div>}

      {!isLoggedIn ? (
        <form onSubmit={handleLogin} style={{ maxWidth: 320 }}>
          <h2>Đăng nhập</h2>
          <input value={username} onChange={(e) => setUsername(e.target.value)} placeholder="Username" style={{ display: 'block', width: '100%', marginBottom: 8 }} />
          <input value={password} onChange={(e) => setPassword(e.target.value)} type="password" placeholder="Password" style={{ display: 'block', width: '100%', marginBottom: 8 }} />
          <button type="submit">Login</button>
        </form>
      ) : (
        <>
          <div style={{ marginBottom: 16 }}>
            <button onClick={handleLogout}>Logout</button>
          </div>

          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 24 }}>
            <section>
              <h2>Customer</h2>
              <form onSubmit={handleCustomerSubmit}>
                <input value={customerForm.name} onChange={(e) => setCustomerForm({ ...customerForm, name: e.target.value })} placeholder="Name" style={{ display: 'block', width: '100%', marginBottom: 8 }} />
                <input value={customerForm.email} onChange={(e) => setCustomerForm({ ...customerForm, email: e.target.value })} placeholder="Email" style={{ display: 'block', width: '100%', marginBottom: 8 }} />
                <input value={customerForm.phone} onChange={(e) => setCustomerForm({ ...customerForm, phone: e.target.value })} placeholder="Phone" style={{ display: 'block', width: '100%', marginBottom: 8 }} />
                <input value={customerForm.address} onChange={(e) => setCustomerForm({ ...customerForm, address: e.target.value })} placeholder="Address" style={{ display: 'block', width: '100%', marginBottom: 8 }} />
                <button type="submit">{customerForm.id ? 'Update Customer' : 'Add Customer'}</button>
              </form>

              <ul>
                {customers.map((customer) => (
                  <li key={customer.id} style={{ marginBottom: 8 }}>
                    <strong>{customer.name}</strong> - {customer.email}
                    <button onClick={() => setCustomerForm(customer)} style={{ marginLeft: 8 }}>Edit</button>
                    <button onClick={() => handleDeleteCustomer(customer.id)} style={{ marginLeft: 8 }}>Delete</button>
                  </li>
                ))}
              </ul>
            </section>

            <section>
              <h2>Order</h2>
              <form onSubmit={handleOrderSubmit}>
                <input value={orderForm.customerId} onChange={(e) => setOrderForm({ ...orderForm, customerId: Number(e.target.value) })} placeholder="Customer Id" style={{ display: 'block', width: '100%', marginBottom: 8 }} />
                <input value={orderForm.orderDate} onChange={(e) => setOrderForm({ ...orderForm, orderDate: e.target.value })} type="datetime-local" style={{ display: 'block', width: '100%', marginBottom: 8 }} />
                <input value={orderForm.totalAmount} onChange={(e) => setOrderForm({ ...orderForm, totalAmount: e.target.value })} placeholder="Total Amount" style={{ display: 'block', width: '100%', marginBottom: 8 }} />
                <input value={orderForm.status} onChange={(e) => setOrderForm({ ...orderForm, status: e.target.value })} placeholder="Status" style={{ display: 'block', width: '100%', marginBottom: 8 }} />
                <button type="submit">{orderForm.id ? 'Update Order' : 'Add Order'}</button>
              </form>

              <ul>
                {orders.map((order) => (
                  <li key={order.id} style={{ marginBottom: 8 }}>
                    <strong>Order #{order.id}</strong> - {order.status} - {order.totalAmount}
                    <button onClick={() => setOrderForm({ ...orderForm, id: order.id, customerId: order.customerId, orderDate: order.orderDate?.slice(0, 16) || '', totalAmount: order.totalAmount, status: order.status })} style={{ marginLeft: 8 }}>Edit</button>
                    <button onClick={() => handleDeleteOrder(order.id)} style={{ marginLeft: 8 }}>Delete</button>
                  </li>
                ))}
              </ul>
            </section>
          </div>
        </>
      )}
    </div>
  );
}

export default App;
