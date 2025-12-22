import React from "react";
import AuthGate from "./AuthGate";
import "../styles/global.css";

const App: React.FC = () => {
	return (
		<div className="app-shell">
			<AuthGate />
		</div>
	);
};

export default App;
