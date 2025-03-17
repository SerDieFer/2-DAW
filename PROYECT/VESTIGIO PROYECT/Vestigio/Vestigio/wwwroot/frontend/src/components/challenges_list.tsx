import { useState, useEffect } from 'react';
import axios from 'axios';
import { Challenge } from '../models/types';

const ChallengeList = () => {
    const [challenges, setChallenges] = useState<Challenge[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchChallenges = async () => {
            try {
                const response = await axios.get('/api/showcase/getchallenges');
                setChallenges(response.data);
                setLoading(false);
            } catch (error) {
                console.error('Error fetching challenges', error);
                setLoading(false);
            }
        };

        fetchChallenges();
    }, []);

    if (loading) {
        return <div>Loading...</div>;
    }

    return (
        <div>
            <h2>Challenges</h2>
            <div className="card-deck">
                {challenges.map(challenge => (
                    <div key={challenge.id} className="card">
                        <img src={challenge.images[0]} alt={challenge.title} />
                        <div className="card-body">
                            <h5 className="card-title">{challenge.title}</h5>
                            <p className="card-text">{challenge.description}</p>
                            <button className="btn btn-primary">Solve Challenge</button>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
};

export default ChallengeList;
