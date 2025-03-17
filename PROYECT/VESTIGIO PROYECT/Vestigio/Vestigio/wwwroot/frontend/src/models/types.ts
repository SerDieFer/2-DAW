
export interface Challenge {
    id: number;
    title: string;
    description: string;
    rarityLevel: number;
    solutionMode: string;
    expPoints: number;
    coins: number;
    password?: string;
    isActive: boolean;
    isPublic: boolean;
    productId?: number;
    productLevel?: number;
    creationDate: string;
    product?: Product;
    images: Image[];
  }
  
  export interface Product {
    id: number;
    name: string;
    description: string;
    price: number;
    rarityLevel: number;
    isActive: boolean;
    creationDate: string;
    images: Image[];
    productSizes: ProductSize[];
    productCategories: ProductCategory[];
  }
  
  export interface Image {
    id: number;
    imageUrl: string;
    challengeId?: number;
    productId?: number;
    isMain: boolean;
  }
  
  export interface ProductSize {
    id: number;
    productId: number;
    size: string;
    stock: number;
  }
  
  export interface ProductCategory {
    productId: number;
    categoryId: number;
    category: Category;
  }
  
  export interface Category {
    id: number;
    name: string;
    description?: string;
  }
  
  export interface ChallengeFilters {
    minLevel?: number;
    maxLevel?: number;
    minXP?: number;
    maxXP?: number;
    minCoins?: number;
    maxCoins?: number;
    solutionType?: string;
    showSolved?: boolean;
    sort?: string;
  }
  
  export interface ProductFilters {
    minProductLevel?: number;
    maxProductLevel?: number;
    minPrice?: number;
    maxPrice?: number;
    categories?: number[];
    sizes?: string[];
    sort?: string;
  }
  
  export interface FilterRanges {
    challengeFilters: {
      minLevel: number;
      maxLevel: number;
      minXP: number;
      maxXP: number;
      minCoins: number;
      maxCoins: number;
      solutionTypes: { value: string; label: string }[];
    };
    productFilters: {
      minPrice: number;
      maxPrice: number;
      minLevel: number;
      maxLevel: number;
      categories: Category[];
      sizes: string[];
    };
  }