# 🧾 Sistema de Pedidos

> 💻 Desenvolvido em **C# / .NET Framework 4.6**  
> 🪟 Utilizado **Visual Studio Community 2019** (necessário devido à compatibilidade com o .NET 4.6)  
> 📦 Dependência: **Newtonsoft.Json** instalada via **NuGet**  
> 🔗 Instalação do Visual Studio 2019 (offline): [Stack Overflow - Guia de Instalação](https://stackoverflow.com/questions/66632243/how-to-download-visual-studio-2019-offline-installer/78164030#78164030)

---

O **Sistema de Pedidos** é uma aplicação desenvolvida para gerenciar **pessoas**, **produtos** e **pedidos** de forma simples e eficiente.  
O sistema permite cadastrar, atualizar, inativar e excluir registros, além de controlar o fluxo de status dos pedidos conforme as regras de negócio.

---

## 🚀 Funcionalidades Principais

### 👤 **Pessoas**
- Cadastrar novas pessoas.  
- Atualizar informações.  
- Alterar status (**Ativo / Inativo**).  
- Excluir registros (desde que não estejam vinculados a pedidos).  
- Filtrar e limpar filtros de busca.  
- Ao clicar em uma pessoa, é possível:
  - Alterar o status.
  - Excluir.
  - Visualizar apenas os pedidos daquela pessoa (botão **Pedidos**).

---

### 📦 **Produtos**
- Inserir novos produtos.  
- Atualizar informações.  
- Alterar status (**Ativo / Inativo**).  
- Excluir produtos (desde que não estejam vinculados a pedidos).  
- Filtrar e limpar filtros de busca.

---

### 🛒 **Pedidos**
- Inserir novos pedidos (somente para pessoas **ativas** e produtos **ativos**).  
- Atualizar pedidos enquanto estiverem **pendentes**.  
- Alterar status dos pedidos.  
- Excluir pedidos.  
- Visualizar detalhes dos produtos de um pedido (botão **Detalhar**).  
- Filtrar e limpar filtros de busca.

---

## 🔁 Fluxo de Status do Pedido

O status do pedido segue um fluxo **sequencial e irreversível**:

```
Pendente → Pago → Enviado → Entregue
```

- Não é permitido alterar o status fora dessa sequência.  
- Não é possível **reverter** o status para um anterior.  
- O status é alterado selecionando a linha do pedido e clicando em **Alterar Status**.

---

## ⚙️ Regras de Negócio

- **Pedidos** só podem ser criados para:
  - **Pessoas** cadastradas e **ativas**.
  - **Produtos** cadastrados e **ativos**.
- **Pessoas** e **produtos** vinculados a pedidos **não podem ser excluídos**.
  - Recomenda-se **inativar** ou **excluir os pedidos** antes.
- A opção de **edição do pedido** só está disponível enquanto o status for **Pendente**.

---

## 🔍 Filtros e Buscas

Em todas as telas:
- Use o botão **Filtrar** para aplicar os filtros de busca.  
- Use o botão **Limpar** para remover os filtros e exibir todos os registros.

---

## 🧠 Navegação entre Telas

- **Tela de Pessoas** → Clique em **Pedidos** para abrir apenas os pedidos daquela pessoa.  
- **Tela de Pedidos** → Clique em **Detalhar** para visualizar os produtos de um pedido.

---

## 🧩 Tecnologias Utilizadas

- 💻 **Linguagem:** C#  
- 🧱 **Framework:** .NET Framework 4.6  
- 🪟 **IDE:** Visual Studio Community 2019  
- 📦 **Dependência:** Newtonsoft.Json (via NuGet)  
- 💾 **Banco de Dados / Persistência:** JSON (armazenamento local)  
- 🧠 **Arquitetura:** Separação entre **Models**, **ViewModels**, **Repositories** e **Views**  
- 🎨 **Interface:** WPF com padrão **MVVM**

---

## 🏗️ Estrutura do Projeto

```
📂 SistemaDePedidos
 ┣ 📂 Models
 ┣ 📂 ViewModels
 ┣ 📂 Views
 ┣ 📂 Resources
 ┣ 📂 Services
 ┗ 📄 README.md
```

---

## 🧰 Como Executar

1. **Clone o repositório:**
   ```bash
   git clone https://github.com/seuusuario/sistema-de-pedidos.git
   ```

2. **Abra o projeto no Visual Studio Community 2019.**

3. **Execute a aplicação:**
   - Pressione **F5** ou clique em **Start**.

---

## 📜 Licença

Este projeto é de uso livre para fins educacionais e pode ser adaptado conforme necessário.

---

## 👨‍💻 Autor

**Seu Nome**  
📧 marcosmarcospaulo42@gmai.com
🔗 [LinkedIn](www.linkedin.com/in/marcos-paulo-429a361b7) | [GitHub](https://github.com/MarckusP)
