import GlobalStyle from "./components/styles/GlobalStyles.styled";
import { ThemeProvider } from "styled-components";
import GameLoginPage from "./pages/GameLoginPage";

const theme = {
  colors: {
    body: "#fff",
  },
  mobile: "768px",
};

function App() {
  return (
    <ThemeProvider theme={theme}>
      <GlobalStyle />
      <GameLoginPage></GameLoginPage>
    </ThemeProvider>
  );
}

export default App;
