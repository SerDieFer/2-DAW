import { useState } from 'react';
import ChallengeList from './challenges_list';
import ProductList from './products_list';

const Showcase = () => {
  const [activeTab, setActiveTab] = useState('challenges');

  return (
    <div>
      <nav>
        <ul>
          <li onClick={() => setActiveTab('challenges')}>Desafíos</li>
          <li onClick={() => setActiveTab('products')}>Productos</li>
        </ul>
      </nav>

      {activeTab === 'challenges' && <ChallengeList />}
      {activeTab === 'products' && <ProductList />}
    </div>
  );
};

export default Showcase;
