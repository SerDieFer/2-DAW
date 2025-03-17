// src/components/Filters.tsx
import React from 'react';

interface FiltersProps {
    onApplyFilters: (filters: { minLevel: number; maxLevel: number; minXP: number; maxXP: number; minCoins: number; maxCoins: number; solutionType: string; }) => void;
}

const Filters: React.FC<FiltersProps> = ({ onApplyFilters }) => {
    const handleApplyFilters = () => {
        const filters = {
            minLevel: 1,
            maxLevel: 10,
            minXP: 100,
            maxXP: 1000,
            minCoins: 10,
            maxCoins: 100,
            solutionType: 'Password',
        };
        onApplyFilters(filters);
    };

    return (
        <div>
            <h3>Filters</h3>
            <button onClick={handleApplyFilters} className="btn btn-secondary">Apply Filters</button>
        </div>
    );
};

export default Filters;
