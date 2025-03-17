import React from 'react';

const CardChallenge = ({ challenge, isSolved, isUnlockable, onSolve }) => {
  return (
    <div className="card">
      <div className="card-header">
        <h3>{challenge.title}</h3>
        <span className={`badge ${isSolved ? 'solved' : 'unsolved'}`}>
          {isSolved ? 'Resuelto' : 'No Resuelto'}
        </span>
      </div>
      <div className="card-body">
        <p>{challenge.description}</p>
        <p><strong>Exp: </strong>{challenge.expPoints}</p>
        <p><strong>Coins: </strong>{challenge.coins}</p>
        {isUnlockable ? (
          <button onClick={() => onSolve(challenge.id)} className="btn btn-primary">
            Resolver desafío
          </button>
        ) : (
          <span>No desbloqueado</span>
        )}
      </div>
      <div className="card-footer">
        {challenge.images && challenge.images.length > 0 && (
          <img src={challenge.images[0]} alt="Challenge" className="challenge-image" />
        )}
      </div>
    </div>
  );
};

export default CardChallenge;
