import styled from "styled-components";

export const MainContainer = styled.div`
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  background-color: #181919;
`;

export const ConnectButton = styled.div`
  background-color: #8ec5fc;
  background-image: linear-gradient(62deg, #8ec5fc 0%, #e0c3fc 100%);
  padding: 1rem;
  color: white;
  border-radius: 10px;
  margin-bottom: 1rem;
  &:hover {
    cursor: pointer;
    color: #181919;
  }
`;
