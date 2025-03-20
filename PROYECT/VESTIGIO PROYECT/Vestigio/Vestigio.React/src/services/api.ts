import axios from "axios";

const API_BASE_URL = "https://tudominio.com/api"; // Cambia esto por la URL real de tu API

const api = axios.create({
  baseURL: API_BASE_URL,
  withCredentials: true, // Si usas autenticación con cookies
});

// Tipos de Datos
export interface Challenge {
  id: number;
  title: string;
  description: string;
  rarityLevel: number;
  expPoints: number;
  coins: number;
  isSolved: boolean;
  isUnlockable: boolean;
  isPublic: boolean;
  images: string[];
}

export interface Product {
  id: number;
  name: string;
  description: string;
  price: number;
  rarityLevel: number;
  images: string[];
  sizes: { id: number; size: string; stock: number }[];
  categories: { id: number; name: string }[];
}

// Obtener desafíos con filtros
export const getChallenges = async (filters = {}) => {
  const response = await api.get<Challenge[]>("/Showcase/GetChallenges", { params: filters });
  return response.data;
};

// Obtener productos con filtros
export const getProducts = async (filters = {}) => {
  const response = await api.get<Product[]>("/Showcase/GetProducts", { params: filters });
  return response.data;
};

// Obtener información del usuario
export const getUserInfo = async () => {
  const response = await api.get<{ isAuthenticated: boolean; level: number; hasUnlockedProducts: boolean }>("/Showcase/GetUserInfo");
  return response.data;
};
