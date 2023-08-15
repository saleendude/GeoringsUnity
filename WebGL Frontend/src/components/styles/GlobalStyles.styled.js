import { createGlobalStyle } from "styled-components";

const GlobalStyle = createGlobalStyle`

* {
    box-sizing: border-box;
    margin: 0;
    padding: 0;
}

  ::-webkit-scrollbar {
    display: none;
}
html {
    height: 100%;
  }

  body {
    height: 100%;
  }

  #root, .App {
    height: 100%;
  }


`;

export default GlobalStyle;
