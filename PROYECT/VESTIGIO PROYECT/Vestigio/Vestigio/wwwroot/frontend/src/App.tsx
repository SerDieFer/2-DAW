import TabSwitcher from './components/tabs.tsx';
import Filters from './components/filters.tsx';

const App = () => {
    return (
        <div className="container mt-5">
            <h1>Welcome to the Store</h1>
            <Filters onApplyFilters={(filters) => console.log(filters)} />
            <TabSwitcher />
        </div>
    );
};

export default App;
