import React from 'react';
import './App.css';
import { Routes, Route } from "react-router-dom";
import { 
  Layout,
  MatchQueue,
  Stats,
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
            <Route path="*" element={<NotFound />} />
          </Route>
        </Routes>
      </div>
  );
}

export default App;
