const API_URL = 'http://localhost:5000';

function getAuthHeaders() {
  const token = localStorage.getItem('token');
  return {
    'Content-Type': 'application/json',
    ...(token ? { Authorization: `Bearer ${token}` } : {})
  };
}

export async function login(username, password) {
  const response = await fetch(`${API_URL}/api/auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ username, password })
  });

  if (!response.ok) throw new Error('Login failed');
  const data = await response.json();
  localStorage.setItem('token', data.token);
  return data;
}

export async function logout() {
  localStorage.removeItem('token');
}

export async function getCustomers() {
  const response = await fetch(`${API_URL}/api/customers`, { headers: getAuthHeaders() });
  if (!response.ok) throw new Error('Failed to fetch customers');
  return response.json();
}

export async function createCustomer(customer) {
  const response = await fetch(`${API_URL}/api/customers`, {
    method: 'POST',
    headers: getAuthHeaders(),
    body: JSON.stringify(customer)
  });
  if (!response.ok) throw new Error('Failed to create customer');
  return response.json();
}

export async function updateCustomer(id, customer) {
  const response = await fetch(`${API_URL}/api/customers/${id}`, {
    method: 'PUT',
    headers: getAuthHeaders(),
    body: JSON.stringify(customer)
  });
  if (!response.ok) throw new Error('Failed to update customer');
  return response.json();
}

export async function deleteCustomer(id) {
  const response = await fetch(`${API_URL}/api/customers/${id}`, {
    method: 'DELETE',
    headers: getAuthHeaders()
  });
  if (!response.ok) throw new Error('Failed to delete customer');
}

export async function getOrders() {
  const response = await fetch(`${API_URL}/api/orders`, { headers: getAuthHeaders() });
  if (!response.ok) throw new Error('Failed to fetch orders');
  return response.json();
}

export async function createOrder(order) {
  const response = await fetch(`${API_URL}/api/orders`, {
    method: 'POST',
    headers: getAuthHeaders(),
    body: JSON.stringify(order)
  });
  if (!response.ok) throw new Error('Failed to create order');
  return response.json();
}

export async function updateOrder(id, order) {
  const response = await fetch(`${API_URL}/api/orders/${id}`, {
    method: 'PUT',
    headers: getAuthHeaders(),
    body: JSON.stringify(order)
  });
  if (!response.ok) throw new Error('Failed to update order');
  return response.json();
}

export async function deleteOrder(id) {
  const response = await fetch(`${API_URL}/api/orders/${id}`, {
    method: 'DELETE',
    headers: getAuthHeaders()
  });
  if (!response.ok) throw new Error('Failed to delete order');
}
