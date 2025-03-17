import { useState } from 'react';
import ChallengeList from './challenges_list';
import ProductList from './products_list';

const TabSwitcher = () => {
    const [activeTab, setActiveTab] = useState('challenges');

    const handleTabChange = (tab: string) => {
        setActiveTab(tab);
    };

    return (
        <div>
            <div className="nav nav-tabs">
                <a
                    className={`nav-item nav-link ${activeTab === 'challenges' ? 'active' : ''}`}
                    onClick={() => handleTabChange('challenges')}
                >
                    Challenges
                </a>
                <a
                    className={`nav-item nav-link ${activeTab === 'products' ? 'active' : ''}`}
                    onClick={() => handleTabChange('products')}
                >
                    Products
                </a>
            </div>

            <div className="tab-content">
                {activeTab === 'challenges' && <ChallengeList />}
                {activeTab === 'products' && <ProductList />}
            </div>
        </div>
    );
};

export default TabSwitcher;
