import React from 'react';
import './App.css';
import { Routes, Route } from "react-router-dom";
import { 
  Layout,
  MatchQueue,
  Stats,
  MatchHistory,
  NotFound,
  Account
} from "./pages";
import { RouteConstants } from "./constants";

function App() {
  return (
      <div>
        <Routes>
          <Route path="/" element={<Layout />}>
            <Route index element={<MatchQueue />} />
            <Route path={RouteConstants.Stats} element={<Stats />} />
            <Route path={RouteConstants.Account} element={<Account />} />
            <Route path={RouteConstants.MatchHistory} element={<MatchHistory />} />
            <Route path="*" element={<NotFound />} />
          </Route>
        </Routes>
      </div>
  );
}

export default App;
