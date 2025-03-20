import React, { useState } from 'react';
import { ChallengeFilters, ProductFilters } from '../models/types';

interface FiltersProps {
    tab: 'challenges' | 'products'; // Determina si estamos en "desafíos" o "productos"
    onApplyFilters: (filters: ChallengeFilters | ProductFilters) => void;
}

const Filters: React.FC<FiltersProps> = ({ tab, onApplyFilters }) => {
    // Filtros comunes para ambos
    const [minLevel, setMinLevel] = useState<number>(1);
    const [maxLevel, setMaxLevel] = useState<number>(10);

    // Filtros específicos de desafíos
    const [minXP, setMinXP] = useState<number>(100);
    const [maxXP, setMaxXP] = useState<number>(1000);
    const [minCoins, setMinCoins] = useState<number>(10);
    const [maxCoins, setMaxCoins] = useState<number>(100);
    const [solutionType, setSolutionType] = useState<string>('Password');

    // Filtros específicos de productos
    const [minPrice, setMinPrice] = useState<number>(10);
    const [maxPrice, setMaxPrice] = useState<number>(100);
    const [categories, setCategories] = useState<number[]>([]); // Ejemplo de categorías
    const [sizes, setSizes] = useState<string[]>([]); // Ejemplo de tamaños

    const handleApplyFilters = () => {
        const commonFilters = {
            minLevel,
            maxLevel,
        };

        if (tab === 'challenges') {
            const challengeFilters: ChallengeFilters = {
                ...commonFilters,
                minXP,
                maxXP,
                minCoins,
                maxCoins,
                solutionType,
            };
            onApplyFilters(challengeFilters);
        } else if (tab === 'products') {
            const productFilters: ProductFilters = {
                ...commonFilters,
                minPrice,
                maxPrice,
                categories,
                sizes,
            };
            onApplyFilters(productFilters);
        }
    };

    return (
        <div>
            <h3>Filters</h3>

            {/* Filtros comunes */}
            <div>
                <label>Min Level: </label>
                <input type="number" value={minLevel} onChange={e => setMinLevel(Number(e.target.value))} />
                <label>Max Level: </label>
                <input type="number" value={maxLevel} onChange={e => setMaxLevel(Number(e.target.value))} />
            </div>

            {tab === 'challenges' && (
                <>
                    {/* Filtros específicos de desafíos */}
                    <div>
                        <label>Min XP: </label>
                        <input type="number" value={minXP} onChange={e => setMinXP(Number(e.target.value))} />
                        <label>Max XP: </label>
                        <input type="number" value={maxXP} onChange={e => setMaxXP(Number(e.target.value))} />
                    </div>

                    <div>
                        <label>Min Coins: </label>
                        <input type="number" value={minCoins} onChange={e => setMinCoins(Number(e.target.value))} />
                        <label>Max Coins: </label>
                        <input type="number" value={maxCoins} onChange={e => setMaxCoins(Number(e.target.value))} />
                    </div>

                    <div>
                        <label>Solution Type: </label>
                        <select value={solutionType} onChange={e => setSolutionType(e.target.value)}>
                            <option value="Password">Password</option>
                            <option value="Puzzle">Puzzle</option>
                            <option value="Quiz">Quiz</option>
                        </select>
                    </div>
                </>
            )}

            {tab === 'products' && (
                <>
                    {/* Filtros específicos de productos */}
                    <div>
                        <label>Min Price: </label>
                        <input type="number" value={minPrice} onChange={e => setMinPrice(Number(e.target.value))} />
                        <label>Max Price: </label>
                        <input type="number" value={maxPrice} onChange={e => setMaxPrice(Number(e.target.value))} />
                    </div>

                    <div>
                        <label>Categories: </label>
                        <input type="text" value={categories.join(', ')} onChange={e => setCategories(e.target.value.split(',').map(Number))} />
                    </div>

                    <div>
                        <label>Sizes: </label>
                        <input type="text" value={sizes.join(', ')} onChange={e => setSizes(e.target.value.split(','))} />
                    </div>
                </>
            )}

            <button onClick={handleApplyFilters} className="btn btn-secondary">Apply Filters</button>
        </div>
    );
};

export default Filters;
