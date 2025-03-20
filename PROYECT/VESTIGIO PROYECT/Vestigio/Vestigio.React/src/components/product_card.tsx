import React from 'react';

interface Product {
  name: string;
  rarityLevel: string;
  description: string;
  price: number;
  sizes: { id: number; size: string; stock: number }[];
  images?: string[];
}

const CardProduct: React.FC<{ product: Product }> = ({ product }) => {
  return (
    <div className="card">
      <div className="card-header">
        <h3>{product.name}</h3>
        <span className="badge">{product.rarityLevel} - Raridad</span>
      </div>
      <div className="card-body">
        <p>{product.description}</p>
        <p><strong>Precio: </strong>{product.price} €</p>
        <p><strong>Stock:</strong> {product.sizes.map(size => (
          <span key={size.id}>{size.size} ({size.stock} disponibles) </span>
        ))}</p>
      </div>
      <div className="card-footer">
        {product.images && product.images.length > 0 && (
          <img src={product.images[0]} alt="Product" className="product-image" />
        )}
      </div>
    </div>
  );
};

export default CardProduct;
